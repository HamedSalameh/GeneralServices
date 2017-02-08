using GeneralServices.Helpers;
using GeneralServices.Interfaces;
using GeneralServices.Models;
using System;
using System.Collections;
using System.Reflection;
using static GeneralServices.Enums;

namespace GeneralServices.Services
{
    public class HistoryService : IHistoryService
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

        public void CreateHistoryEntry(int EntityID, int? EntityOwnerID, int EntityTypeID, int ActionUserID, CRUDType CRUDType)
        {
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

            HistoryServiceDBHelper.AddEntityHistoryEntry(entityHistoryEntry, ConnectionString);
        }

        public void CreateHistoryEntry(int EntityID, int? EntityOwnerID, EntityTypeLookup EntityTypeID, int ActionUserID, CRUDType CRUDType)
        {
            HistoryLog entityHistoryEntry = new HistoryLog();
            entityHistoryEntry.CRUDType = CRUDType;
            entityHistoryEntry.Date = DateTime.Now;
            entityHistoryEntry.EntityID = EntityID;
            entityHistoryEntry.EntityOwnerID = EntityOwnerID != null ? EntityOwnerID.Value : 0;
            entityHistoryEntry.EntityTypeID = EntityTypeID.EntityTypeID;

            entityHistoryEntry.HashID = entityHistoryEntry.GetHashCode();

            HistoryServiceDBHelper.AddEntityHistoryEntry(entityHistoryEntry, ConnectionString);
        }

        public void CreateHistoryPropertyChangeEntry(IEntity Entity)
        {
            Hashtable hash = new Hashtable();
            // Parse the entity properties using refletion
            PropertyInfo[] pInfo = Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            // get each property it's EntityPropertyID from EntityPropertyLookup
            // compare the properties in the list with their values on the DB
        }

        public void GetDomainEntityPropertyLookupTable()
        {

        }
    }
}
