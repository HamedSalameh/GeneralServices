using System;
using System.ComponentModel.DataAnnotations;
using static GeneralServices.Enums;

namespace GeneralServices.Models
{
    public partial class HistoryLog
    {
        [Required]
        public virtual int HistoryLogID { get; set; }

        /// <summary>
        /// The type of the domain model entity that is connected to this event entry
        /// </summary>
        [Required]
        public virtual int EntityTypeID { get; set; }

        /// <summary>
        /// ID of the domain model entity that is connected to this event entry
        /// </summary>
        [Required]
        public virtual int EntityID { get; set; }

        public virtual int? EntityOwnerID { get; set; }

        [Required]
        public virtual DateTime Date { get; set; }

        [Required]
        public virtual CRUDType CRUDType { get; set; }

        public virtual int HashID { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
