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
        public void CreateHistoryEntry(int EntityID, int? EntityOwnerID, EntityTypeLookup EntityTypeID, string EntityDisplayText, int ActionUserID, CRUDType CRUDType)
        {
            HistoryLog entityHistoryEntry = new HistoryLog();
            entityHistoryEntry.CRUDType = CRUDType;
            entityHistoryEntry.Date = DateTime.Now;
            entityHistoryEntry.EntityID = EntityID;
            entityHistoryEntry.EntityOwnerID = EntityOwnerID.Value;
            entityHistoryEntry.EntityTypeID = EntityTypeID;
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
