﻿using GeneralServices.Models;
using static GeneralServices.Enums;

namespace GeneralServices.Interfaces
{
    public interface IHistoryService
    {
        void CreateHistoryEntry(int EntityID, int? EntityOwnerID, int EntityTypeID, int ActionUserID, CRUDType CRUDType);
        void CreateHistoryEntry(int EntityID, int? EntityOwnerID, EntityTypeLookup EntityTypeID, int ActionUserID, CRUDType CRUDType);

        void CreateHistoryPropertyChangeEntry(IEntity Entity);

        void GetDomainEntityPropertyLookupTable();
    }
}
