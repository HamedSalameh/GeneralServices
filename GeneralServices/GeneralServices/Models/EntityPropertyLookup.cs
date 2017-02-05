using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeneralServices.Models
{
    /// <summary>
    /// A class representing a property in a domain model class (each property will be given a unique hash code)
    /// <para>
    /// EntityPropertyID:-1264412556	 (hash) EntityPropertyName: "AmountUsed"	EntityTypeID: 130271153 (FK to EntityTypeLookup)
    /// </para>
    /// </summary>
    public partial class EntityPropertyLookup
    {
        [Required]
        public virtual int ID { get; set; }

        [Required]
        public virtual int EntityPropertyID { get; set; }

        [Required]
        public virtual string EntityPropertyName { get; set; }

        // FK to EntityTypeLookupID
        // FK
        [Required]
        public virtual int EntityTypeLookupID { get; set; }

        [Required]
        [ForeignKey("EntityTypeLookupID")]
        public EntityTypeLookup EntityType { get; set; }
    }
}
