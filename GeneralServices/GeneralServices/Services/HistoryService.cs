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
        private static HistoryService _instance;
        private static string _connectionString;

        private HistoryService()
        {
            // do all initalizations in the CTOR
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

        public void CreateHistoryEntry(int EntityID, int? EntityOwnerID, EntityTypeLookup EntityTypeID, string EntityDisplayText, int ActionUserID, CRUDType CRUDType)
        {
            HistoryLog entityHistoryEntry = new HistoryLog();
            entityHistoryEntry.CRUDType = CRUDType;
            entityHistoryEntry.Date = DateTime.Now;
            entityHistoryEntry.EntityID = EntityID;
            entityHistoryEntry.EntityOwnerID = EntityOwnerID.Value;
            entityHistoryEntry.EntityTypeID = EntityTypeID;

            entityHistoryEntry.HashID = entityHistoryEntry.GetHashCode();

            HistoryServiceDBHelper.AddEntityHistoryEntry(entityHistoryEntry);
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
