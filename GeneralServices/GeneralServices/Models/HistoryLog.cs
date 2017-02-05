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
        /// The type of the domain model entity that we want to log event for
        /// </summary>
        [Required]
        public virtual EntityTypeLookup EntityTypeID { get; set; }

        /// <summary>
        /// ID of the domain model entity that we need to log a history event for
        /// </summary>
        [Required]
        public virtual int EntityID { get; set; }

        public virtual int? EntityOwnerID { get; set; }

        public virtual string EntityDisplayText { get; set; }

        [Required]
        public virtual DateTime Date { get; set; }

        /// <summary>
        /// ID of the user that is associated with the event we want to log (e.g, who made this action)
        /// </summary>
        [Required]
        public virtual int ActionUserID { get; set; }

        [Required]
        public virtual CRUDType CRUDType { get; set; }
    }
}
