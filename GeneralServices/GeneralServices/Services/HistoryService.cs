using GeneralServices.Helpers;
using GeneralServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeneralServices.Enums;

namespace GeneralServices.Services
{
    public class HistoryService
    {
        #region Private properties
        private static HistoryService _instance;
        private static string _connectionString;
        private static bool _historyServiceInitialized;
        #endregion

        #region Public Properties
        public string ConnectionString
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception(string.Format("{0} : ConnectionString value cannot be null or empty", Reflection.GetCurrentMethodName()));
                }
                _connectionString = value;
            }
            get
            {
                return _connectionString;
            }
        }
        public bool IsHistoryServiceInitiazlied
        {
            get
            {
                return _historyServiceInitialized;
            }
        }
        #endregion

        #region Singleton contructor
        private HistoryService()
        {
            // do all initalizations here
            _connectionString = string.Empty;
            _historyServiceInitialized = false;
        }

        public static HistoryService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HistoryService();
                }
                return _instance;
            }
        }
        #endregion

        #region Private Methods
        private int CreateEntityHistoryEntry(int EntityID, int? EntityOwnerID, int EntityTypeID, int ActionUserID, CRUDType CRUDType)
        {
            int historyLogID = Consts.INVALID_INDEX;
            if (_historyServiceInitialized == false)
            {
                throw new Exception(string.Format("{0} : History log service is not initialized.", Reflection.GetCurrentMethodName()));
            }

            HistoryLog entityHistoryEntry = new HistoryLog();
            entityHistoryEntry.CRUDType = CRUDType;
            entityHistoryEntry.Date = DateTime.Now;
            entityHistoryEntry.EntityID = EntityID;
            entityHistoryEntry.EntityOwnerID = EntityOwnerID != null ? EntityOwnerID.Value : 0;
            entityHistoryEntry.EntityTypeID = EntityTypeID;
            entityHistoryEntry.HashID = entityHistoryEntry.GetHashCode();

            historyLogID = HistoryServiceDBHelper.AddEntityHistoryEntry(entityHistoryEntry, ConnectionString);
            return historyLogID;
        }

        private void CreateEntityPropertyChangesHistoryLogs(List<EntityPropertyChange> Changes, int HistoryLogID)
        {
            if (_historyServiceInitialized == false)
            {
                throw new Exception(string.Format("{0} : History log service is not initialized.", Reflection.GetCurrentMethodName()));
            }

            if (Changes != null && Changes.Count > 0 && HistoryLogID != Consts.INVALID_INDEX)
            {
                Changes.ForEach(c => c.HistoryLogID = HistoryLogID);
                HistoryServiceDBHelper.AddEntityPropertyChangesHistoryLogs(Changes, ConnectionString);
            }
        }
        #endregion


        public bool Initialize()
        {
            bool result = false;

            if (IsHistoryServiceInitiazlied == false)
            {
                try
                {
                    HistoryServiceDBHelper.createHistoryLogTable(ConnectionString);
                    result = HistoryServiceDBHelper.validateHistoryLogTable(ConnectionString);
                    // if main history table exists, check the entity property changes table
                    if (result)
                    {
                        HistoryServiceDBHelper.createEntityPropertyChangesTable(ConnectionString);
                        result = HistoryServiceDBHelper.validateEntityPropertyChangesTable(ConnectionString);
                    }

                    if (result)
                    {
                        HistoryServiceDBHelper.createUDT_EntityPropertChangesTable(ConnectionString);
                        HistoryServiceDBHelper.createUSP_InsertEntityPropertyChanges(ConnectionString);
                    }
                }
                catch (Exception Ex)
                {
                    throw new Exception(string.Format("{0} : Unable to initialize HistorySerivce.{1}", Reflection.GetCurrentMethodName(), Environment.NewLine + Ex.Message), Ex);
                }
                // Init history logs tables, stored procedures and user defined types
                if (result)
                {
                    _historyServiceInitialized = result;
                }
            }

            return result;
        }

        public void CreateHistoryEntry(object NewEntity, int ActionUserID, CRUDType CRUDType)
        {
            if (!IsHistoryServiceInitiazlied)
            {
                throw new Exception(string.Format("{0} : History service is not yet initialized.", Reflection.GetCurrentMethodName()));
            }

            // try detect the ID property for the object
            Type type = NewEntity.GetType();
            var properties = type.GetProperties();
            if (properties != null && properties.Length > 0)
            {
                var idProperty = properties.FirstOrDefault(p => p.Name.ToLower().Equals("id"));
                // try another combination
                if (idProperty == null)
                {
                    idProperty = properties.FirstOrDefault(p => p.Name.ToLower().Equals(type.Name + "id"));
                }

                if (idProperty != null)
                {
                    int id = 0;
                    int.TryParse(idProperty.GetValue(NewEntity).ToString(), out id);

                    if (id != 0)
                    {
                        CreateHistoryEntry(id, NewEntity, ActionUserID, CRUDType);
                    }
                }
            }

        }

        /// <summary>
        /// Creates a new entry in the history table for a domain model entity
        /// </summary>
        /// <param name="EntityID">The entity ID of the domain model entity</param>
        /// <param name="NewEntity">An object holding the new entity object (after the change was comitted to DB)</param>
        /// <param name="ActionUserID">(Optional) The user ID that executed the last change on the domain model entity</param>
        /// <param name="CRUDType">Lookup defining the change type (CREATE, UPDAT or DELETE)</param>
        public void CreateHistoryEntry(int EntityID, object NewEntity, int ActionUserID, CRUDType CRUDType)
        {
            if (!IsHistoryServiceInitiazlied)
            {
                throw new Exception(string.Format("{0} : History service is not yet initialized.", Reflection.GetCurrentMethodName()));
            }

            if (EntityID != 0 && NewEntity != null)
            {
                CreateHistoryEntry(EntityID, null, NewEntity, ActionUserID, CRUDType);
            }
        }

        /// <summary>
        /// Creates a new entry in the history table for a domain model entity
        /// </summary>
        /// <param name="EntityID">The entity ID of the domain model entity</param>
        /// <param name="OldEntity">An object holding the old entity object (before last change)</param>
        /// <param name="NewEntity">An object holding the new entity object (after the change was comitted to DB)</param>
        /// <param name="ActionUserID">(Optional) The user ID that executed the last change on the domain model entity</param>
        /// <param name="CRUDType">Lookup defining the change type (CREATE, UPDAT or DELETE)</param>
        public void CreateHistoryEntry(int EntityID, object OldEntity, object NewEntity, int ActionUserID, CRUDType CRUDType)
        {
            if (!IsHistoryServiceInitiazlied)
            {
                throw new Exception(string.Format("{0} : History service is not yet initialized.", Reflection.GetCurrentMethodName()));
            }

            List<EntityPropertyChange> changes = null;
            try
            {
                if (OldEntity != null && NewEntity != null)
                {
                    changes = Reflection.GetEntityPropertyChanges(OldEntity, NewEntity);
                }
                // special case: new entity, does not have an 'old entity' since it's a newly created one
                else if (OldEntity == null && NewEntity != null && CRUDType == CRUDType.Create)
                {
                    changes = Reflection.GetEntityPropertyValuesAsChanges(NewEntity);
                }
            }
            catch (Exception Ex)
            {
                throw new Exception(string.Format("{0} : Unable to create history entry for entity ID {1} : {2}", Reflection.GetCurrentMethodName(),
                    EntityID, Environment.NewLine + Ex.Message), Ex);
            }

            if (changes != null && changes.Count > 0)
            {
                int _hash = General.calculateClassHash(NewEntity.GetType()).Value;
                // Save history log entry for the entity
                try
                {
                    int historyLogID = CreateEntityHistoryEntry(EntityID, 0, _hash, ActionUserID, CRUDType);
                    // Save history log for the entity property changes
                    if (historyLogID != Consts.INVALID_INDEX)
                    {
                        CreateEntityPropertyChangesHistoryLogs(changes, historyLogID);
                    }
                }
                catch (Exception Ex)
                {
                    throw new Exception(string.Format("{0} : Unable to create history entry for entity ID {1} : {2}", Reflection.GetCurrentMethodName(),
                    EntityID, Environment.NewLine + Ex.Message), Ex);
                }
            }
        }

        public List<HistoryLog> GetEntityHistory(int EntityID)
        {
            if (!IsHistoryServiceInitiazlied)
            {
                throw new Exception(string.Format("{0} : History service is not yet initialized.", Reflection.GetCurrentMethodName()));
            }

            List<HistoryLog> entityHistoryLog = new List<HistoryLog>();

            if (EntityID != 0)
            {
                try
                {
                    entityHistoryLog = HistoryServiceDBHelper.GetEntityHistory(EntityID, ConnectionString).MapEntityHistoryTable();
                }
                catch (Exception Ex)
                {
                    throw new Exception(string.Format("{0} : Unable to get entity change history.{1}", Reflection.GetCurrentMethodName(), Environment.NewLine + Ex.Message), Ex);
                }
            }

            return entityHistoryLog;
        }

        public List<EntityPropertyChange> GetEntityDetailedHistory(int HistoryLogID)
        {
            List<EntityPropertyChange> entityPropertyChanges = null;

            if (!IsHistoryServiceInitiazlied)
            {
                throw new Exception(string.Format("{0} : History service is not yet initialized.", Reflection.GetCurrentMethodName()));
            }

            if (HistoryLogID != 0)
            {
                try
                {
                    entityPropertyChanges = HistoryServiceDBHelper.GetEntityDetailedHistory(HistoryLogID, ConnectionString).MapPropertyChangeTable();
                }
                catch (Exception Ex)
                {
                    throw new Exception(string.Format("{0} : Unable to get entity property changes history.{1}", Reflection.GetCurrentMethodName(), Environment.NewLine + Ex.Message), Ex);
                }
            }

            return entityPropertyChanges;
        }
    }
}
