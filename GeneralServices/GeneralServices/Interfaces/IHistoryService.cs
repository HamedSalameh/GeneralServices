using GeneralServices.Models;
using static GeneralServices.Enums;

namespace GeneralServices.Interfaces
{
    public interface IHistoryService
    {
        void CreateHistoryEntry(int EntityID, int? EntityOwnerID, EntityTypeLookup EntityTypeID, string EntityDisplayText, int ActionUserID, CRUDType CRUDType);

        void CreateHistoryPropertyChangeEntry(IEntity Entity);

        void GetDomainEntityPropertyLookupTable();
    }
}
