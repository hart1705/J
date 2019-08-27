using Microsoft.AspNetCore.Mvc;
using PORTAL.DAL.EF;
using PORTAL.WEB.Models;
using PORTAL.WEB.UserControls.LookupControl;
using System.Linq;

namespace PORTAL.WEB.Controllers
{
    public class LookupControlController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LookupControlController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult LookupRecord(string lookupControlId, string title, string entityName, string primaryKeyName, string columnSet, string recordId)
        {
            string[] columns = columnSet.Split('#');
            string[] tableCols = columns.Select(s => s.Replace(":", " ")).ToArray();
            
            string queryCol = string.Join(',', tableCols);

            var record = Lookup.LoadTableRecords(_context, entityName, primaryKeyName, queryCol, string.Empty, recordId);

            SingleLookupControlModel model = new SingleLookupControlModel
            {
                LookupControlId = lookupControlId,
                Title = title,
                Records = record,
                QueryCol = queryCol,
                EntityName = entityName,
                PrimaryKeyName = primaryKeyName
            };
            return PartialView("_LookupRecord", model);
        }

        public IActionResult LoadViewComponent(string lookupControlId, string title, string primaryRecordId, string relationShipName)
        {
            TableDef tableDef = Lookup.ResolveTableDefinition(relationShipName);
            LookupControlModel model = new LookupControlModel();
            model.Title = title;
            model.LookupControlId = lookupControlId;
            model.PrimaryRecordId = primaryRecordId;
            model.RelationshipName = relationShipName;

            return ViewComponent("LookupControl", new { model });
        }

        public IActionResult LoadRecords(string lookupControlId, string title, string primaryRecordId, string relationShipName, string search = "")
        {
            LookupControlModel lookupModel = new LookupControlModel();
            TableDef tableDef = Lookup.ResolveTableDefinition(relationShipName);
            TableRecord record = null;
            if (tableDef != null)
            {
                record = Lookup.LoadTableRecords(_context, primaryRecordId, tableDef, true, search);
            }
            lookupModel.Title = title;
            lookupModel.LookupControlId = lookupControlId;
            lookupModel.PrimaryRecordId = primaryRecordId;
            lookupModel.RelationshipName = relationShipName;
            lookupModel.Records = record;
            return PartialView("_LookupControlForm", lookupModel);
        }

        public IActionResult SingleLoadRecords(string lookupControlId, string entityName, string primaryKeyName, string queryCol, string title, string search = "")
        {
            SingleLookupControlModel lookupModel = new SingleLookupControlModel();
            TableRecord record = Lookup.LoadTableRecords(_context, entityName, primaryKeyName, queryCol, search);
            
            lookupModel.Title = title;
            lookupModel.LookupControlId = lookupControlId;
            lookupModel.PrimaryKeyName = primaryKeyName;
            lookupModel.QueryCol = queryCol;
            lookupModel.EntityName = entityName;
            lookupModel.Records = record;
            return PartialView("_LookupRecord", lookupModel);
        }

        public IActionResult Associate(LookupControlModel model)
        {
            LookupControlModel lookupModel = new LookupControlModel();
            TableDef tableDef = Lookup.ResolveTableDefinition(model.RelationshipName);
            if (tableDef != null)
            {
                foreach (var selected in model.Records.Rows.Where(r=>r.Selected))
                {
                    bool result = Lookup.AssociateRecord(_context, model.PrimaryRecordId, selected.RecordId, tableDef);
                }
            }
            lookupModel.LookupControlId = model.LookupControlId;
            lookupModel.PrimaryRecordId = model.PrimaryRecordId;
            lookupModel.Title = model.Title;
            lookupModel.RelationshipName = model.RelationshipName;
            return PartialView("_BootstrapModalAction", LookupModalAction(lookupModel));
        }

        public IActionResult Disassociate(string lookupControlId, string title, string primaryRecordId, string recordId, string relationShipName)
        {
            LookupControlModel lookupModel = new LookupControlModel();
            TableDef tableDef = Lookup.ResolveTableDefinition(relationShipName);
            if (tableDef != null)
            {
                bool result = Lookup.DisassociateRecord(_context, primaryRecordId, recordId, tableDef);
            }
            lookupModel.Title = title;
            lookupModel.LookupControlId = lookupControlId;
            lookupModel.PrimaryRecordId = primaryRecordId;
            lookupModel.RelationshipName = relationShipName;
            return ViewComponent("LookupControl", new { @model = lookupModel });
        }
        
        private BootstrapModalModel LookupModalAction(LookupControlModel model)
        {
            return new BootstrapModalModel
            {
                ModalID = $"#{model.LookupControlId}-lookup-modal",
                ShouldClose = true,
                FetchData = true,
                Destination = Url.Action("LoadViewComponent", new { @lookupControlId = model.LookupControlId, @title = model.Title,
                    @primaryRecordId = model.PrimaryRecordId, @relationShipName = model.RelationshipName }),
                Target = $"#{model.LookupControlId}-lookup-container",
                OnSuccess = $"Lookup.BindToDataTable('{model.LookupControlId}-lookup-table')"
            };
        }
    }
}