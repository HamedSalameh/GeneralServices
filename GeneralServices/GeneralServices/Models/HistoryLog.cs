using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
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

        public virtual int ActionUserID { get; set; }

        public virtual int HashID { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class HistoryLogExtentions
    {
        public static HistoryLog MapHistoryRecord(this DataRow Row)
        {
            HistoryLog entry = null;
            int _entityID = -1,_actionUserID = -1, _entityOwnerID = -1, entityTypeLookup = 0, _hashID = 0, _historyLogID = 0;
            CRUDType _crudType;

            if (Row != null)
            {
                DateTime _date = DateTime.Today;
                int.TryParse(Row["EntityID"].ToString(), out _entityID);

                if (_entityID != -1)
                {
                    entry = new HistoryLog();
                    entry.EntityID = _entityID;
                    DateTime.TryParse(Row["Date"].ToString(), out _date);

                    int.TryParse(Row["EntityTypeLookup"].ToString(), out entityTypeLookup);
                    int.TryParse(Row["EntityOwnerID"].ToString(), out _entityOwnerID);
                    Enum.TryParse(Row["CRUDType"].ToString(), out _crudType);
                    int.TryParse(Row["ActionUserID"].ToString(), out _actionUserID);
                    int.TryParse(Row["HashID"].ToString(), out _hashID);
                    int.TryParse(Row["HistoryLogID"].ToString(), out _historyLogID);

                    entry.EntityTypeID = entityTypeLookup;
                    entry.EntityOwnerID = _entityOwnerID;
                    entry.CRUDType = _crudType;
                    entry.Date = _date;
                    entry.HistoryLogID = _historyLogID;
                }
            }
            return entry;
        }

        public static List<HistoryLog> MapEntityHistoryTable(this DataTable Table)
        {
            List<HistoryLog> historyLog = new List<HistoryLog>();

            if (Table != null && Table.Rows != null && Table.Rows.Count > 0)
            {
                foreach(DataRow row in Table.Rows)
                {
                    HistoryLog entry = row.MapHistoryRecord();
                    if (entry != null)
                    {
                        historyLog.Add(entry);
                    }
                }
            }

            return historyLog;
        }
    }
}
