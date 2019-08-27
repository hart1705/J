using PORTAL.WEB.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.UserControls.LookupControl
{
    public class LookupControlModel
    {
        public string LookupControlId { get; set; }
        public string Title { get; set; }
        public string PrimaryRecordId { get; set; }
        public TableRecord Records { get; set; }
        public string RelationshipName { get; set; }
        public bool Enabled { get; set; }
    }

    public class SingleLookupControlModel
    {
        public string LookupControlId { get; set; }
        public string Title { get; set; }
        public TableRecord Records { get; set; }
        public string QueryCol { get; set; }
        public string PrimaryKeyName { get; set; }
        public string EntityName { get; set; }
    }

    public class TableDef
    {
        public string TableDefName { get; set; }
        public string PrimaryEntity { get; set; }
        public string SecondaryEntity { get; set; }
        public string RelationShipName { get; set; }
        public string RecordId { get; set; }
        public List<KeyValuePair<string, string>> ColumnSet { get; set; }
    }

    public class TableRecord
    {
        public string TableId { get; set; }
        public List<string> ColumnHeaders { get; set; }
        public List<RecordRow> Rows { get; set; }
    }

    public class RecordRow
    {
        public string RecordId { get; set; }
        public bool Selected { get; set; }
        public List<string> ColumnValues { get; set; }
    }

}
