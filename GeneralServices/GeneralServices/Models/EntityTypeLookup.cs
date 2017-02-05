using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GeneralServices.Models
{
    /// <summary>
    /// The domain model entity type in FQDN format
    /// <para>2104820528	Eyeblaster.ACM.BusinessEntities.Accounts.AccountBetaModule</para>
    /// <para>123     BudgetMake.Shared.DomainModel.Expense</para>
    /// <para>456     BudgetMake.Shared.DomainModel.Salary</para>
    /// </summary>
    public partial class EntityTypeLookup
    {
        [Required]
        public virtual int ID { get; set; }

        [Required]
        public virtual int EntityTypeID { get; set; }

        [Required]
        public virtual string EntityTypeName { get; set; }

        // FK
        public virtual List<EntityPropertyLookup> EntityProperties { get; set; }
    }
}
