using Microsoft.AspNetCore.Mvc;
using PORTAL.DAL.EF;
using PORTAL.WEB.UserControls.LookupControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PORTAL.WEB.ViewComponents
{
    public class LookupControlViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public LookupControlViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke(LookupControlModel model, bool enabled = true)
        {
            if (model != null)
            {
                var tableDef = Lookup.ResolveTableDefinition(model.RelationshipName);
                var record = Lookup.LoadTableRecords(_context, model.PrimaryRecordId, tableDef);
                model.Records = record;
                model.Enabled = enabled;
            }
            return View(model);
        }
    }
}
