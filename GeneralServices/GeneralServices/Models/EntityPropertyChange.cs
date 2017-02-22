using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace GeneralServices.Models
{
    public class EntityPropertyChange
    {
        public int EntityPropertyChangeID { get; set; }

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

    public static class EntityPropertyChangeExtentions
    {
        public static EntityPropertyChange MapPropertyChangeRecord(this DataRow Row)
        {
            EntityPropertyChange change = null;
            int entityPropertyChangeID = -1,historyLogID = -1, entityPropertyID = 0, hashID = 0;
            string currentValueAsText, originalValueAsText;
            DateTime Date;

            if (Row != null)
            {
                int.TryParse(Row["EntityPropertyChangeID"].ToString(), out entityPropertyChangeID);

                if (entityPropertyChangeID != -1)
                {
                    change = new EntityPropertyChange();

                    int.TryParse(Row["HistoryLogID"].ToString(), out historyLogID);
                    int.TryParse(Row["EntityPropertyID"].ToString(), out entityPropertyID);
                    currentValueAsText = Row["CurrentValueAsText"].ToString();
                    originalValueAsText = Row["OriginalValueAsText"].ToString();
                    DateTime.TryParse(Row["Date"].ToString(), out Date);
                    int.TryParse(Row["HashID"].ToString(), out hashID);

                    change.HistoryLogID = historyLogID;
                    change.EntityPropertyID = entityPropertyID;
                    change.CurrentValueAsText = currentValueAsText;
                    change.OriginalValueAsText = originalValueAsText;
                    change.Date = Date;
                    change.HashID = hashID; 
                }
            }

            return change;
        }

        public static List<EntityPropertyChange> MapPropertyChangeTable(this DataTable Table)
        {
            List<EntityPropertyChange> entityChanges = new List<EntityPropertyChange>();

            if (Table != null && Table.Rows != null && Table.Rows.Count > 0)
            {
                foreach(DataRow row in Table.Rows)
                {
                    EntityPropertyChange change = row.MapPropertyChangeRecord();
                    if(change != null)
                    {
                        entityChanges.Add(change);
                    }
                }
            }

            return entityChanges;
        }
    }
}
