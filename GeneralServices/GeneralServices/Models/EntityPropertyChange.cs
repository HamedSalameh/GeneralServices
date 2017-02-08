using System;
using System.ComponentModel.DataAnnotations;

namespace GeneralServices.Models
{
    public class EntityPropertyChange
    {
        [Required]
        public int HistoryLogID { get; set; }

        [Required]
        public int EntityPropertyID { get; set; }

        public string CurrentValueAsText { get; set; }

        public string OriginalValueAsText { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int HashID { get; set; }
    }
}
