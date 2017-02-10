using GeneralServices.Helpers;
using GeneralServices.Models;
using System;
using System.Collections.Generic;
using static GeneralServices.Enums;

namespace GeneralServices.Services
{
    public class HistoryService
    {
        #region Private properties
        private static HistoryService _instance;
        private static string _connectionString;
        private static bool _historyLogTablesInitialized;
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
        public bool IsHistoryLogTablesInitiazlied
        {
            get
            {
                return _historyLogTablesInitialized;
            }
        } 
        #endregion

        #region Singleton contructor
        private HistoryService()
        {
            // do all initalizations here
            _connectionString = string.Empty;
            _historyLogTablesInitialized = false;
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

        public bool Initialize()
        {
            bool result = false;

            if (IsHistoryLogTablesInitiazlied == false)
            {
                HistoryServiceDBHelper.createHistoryLogTable(ConnectionString);
                result = HistoryServiceDBHelper.validateHistoryLogTable(ConnectionString);
                // if main history table exists, check the entity property changes table
                if (result)
                {
                    HistoryServiceDBHelper.createEntityPropertyChangesTable(ConnectionString);
                    result = HistoryServiceDBHelper.validateEntityPropertyChangesTable(ConnectionString);
                }
                // Init history logs tables, stored procedures and user defined types
                if (result)
                {
                    _historyLogTablesInitialized = result;
                }
            }

            return result;
        }

        private int CreateEntityHistoryEntry(int EntityID, int? EntityOwnerID, int EntityTypeID, int ActionUserID, CRUDType CRUDType)
        {
            int historyLogID = Consts.INVALID_INDEX;
            if (_historyLogTablesInitialized == false)
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
            if (_historyLogTablesInitialized == false)
            {
                throw new Exception(string.Format("{0} : History log service is not initialized.", Reflection.GetCurrentMethodName()));
            }

            if (Changes != null && Changes.Count > 0 && HistoryLogID != Consts.INVALID_INDEX)
            {
                Changes.ForEach(c => c.HistoryLogID = HistoryLogID);
                HistoryServiceDBHelper.AddEntityPropertyChangesHistoryLogs(Changes);
            }
        }

        public void CreateHistoryEntry(int EntityID, object OldEntity, object NewEntity, int ActionUserID, CRUDType CRUDType)
        {
            var changes = Reflection.GetEntityPropertyChanges(OldEntity, NewEntity);
            int _hash = General.calculateClassHash(NewEntity.GetType()).Value;
            // Save history log entry for the entity
            try
            {
                int historyLogID = CreateEntityHistoryEntry(EntityID, 0, _hash, 0, CRUDType);
                // Save history log for the entity property changes
                if (historyLogID != Consts.INVALID_INDEX)
                {
                    CreateEntityPropertyChangesHistoryLogs(changes, historyLogID);
                }
                
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

    }
}
