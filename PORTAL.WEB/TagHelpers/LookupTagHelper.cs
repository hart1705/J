using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PORTAL.DAL.EF;
using PORTAL.WEB.UserControls.LookupControl;

namespace PORTAL.WEB.TagHelpers
{
    [HtmlTargetElement("lookup", Attributes = "input-id, lookup-id, record-id, entity-name, display-column, " +
        "display-columnset, input-size, target-modal, lookup-title")]
    public class LookupTagHelper : TagHelper
    {
        public string InputId { get; set; }
        public string LookupId { get; set; }
        public string RecordId { get; set; }
        public string EntityName { get; set; }
        public string DisplayColumn { get; set; }
        public string InputSize { get; set; }
        public string TargetModal { get; set; }
        public string LookupTitle { get; set; }
        public string DisplayColumnset { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }
        public IUrlHelperFactory _urlHelper { get; set; }
        private readonly ApplicationDbContext _dbContext;

        public LookupTagHelper(ApplicationDbContext dbContext, IUrlHelperFactory urlHelper)
        {
            _dbContext = dbContext;
            _urlHelper = urlHelper;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "text";
            output.Attributes.Add("id", $"{TargetModal}-lookup-maincontrol");
            var displayVal = Lookup.GetSelectedRecord(_dbContext, EntityName, RecordId, DisplayColumn);
            var urlHelper = _urlHelper.GetUrlHelper(ViewContext);
            var container = new TagBuilder("div");
            var span = new TagBuilder("span");
            var icon = new TagBuilder("i");
            var inputHid = new TagBuilder("input");
            var inputDis = new TagBuilder("input");
            var a = new TagBuilder("a");

            container.Attributes.Add("class", $"input-group input-group-{InputSize}");

            span.Attributes.Add("class", "input-group-btn");
            span.InnerHtml.AppendHtml(a);

            icon.Attributes.Add("class", "fa fa-search");

            inputHid.Attributes.Add("id", InputId.Replace('.', '_'));
            inputHid.Attributes.Add("name", InputId);
            inputHid.Attributes.Add("type", "hidden");
            inputHid.Attributes.Add("value", RecordId);

            inputDis.Attributes.Add("type", "text");
            inputDis.Attributes.Add("id", $"{InputId}-lookup");
            inputDis.Attributes.Add("name", $"{InputId}-lookup");
            inputDis.Attributes.Add("disabled", "disabled");
            inputDis.Attributes.Add("class", $"form-control lookup-control");
            inputDis.Attributes.Add("value", displayVal);

            container.InnerHtml.AppendHtml(inputDis);
            container.InnerHtml.AppendHtml(inputHid);
            container.InnerHtml.AppendHtml(span);

            var url = urlHelper.Action("LookupRecord", "LookupControl", 
                new { @lookupControlId = TargetModal, @title = LookupTitle, @columnSet = DisplayColumnset,
                @entityName = EntityName, @primaryKeyName = DisplayColumn, @recordId = RecordId });
            a.MergeAttribute("href", $"{url}");
            a.Attributes.Add("data-target", $"#{TargetModal}");
            a.Attributes.Add("data-ajax", "true");
            a.Attributes.Add("data-ajax-method", "GET");
            a.Attributes.Add("data-ajax-mode", "replace");
            a.Attributes.Add("data-ajax-success", $"Helper.BootstrapModal_OnSuccess(this); Lookup.ModalBindToDataTable('{TargetModal}-lookup-addtable', true);");
            a.Attributes.Add("data-ajax-update", $"#{TargetModal}-content");
            a.Attributes.Add("data-ajax-loading", "#application_loading");
            a.Attributes.Add("data-ajax-loading-duration", "5");
            a.Attributes.Add("class", "btn btn-flat btn-default");
            a.InnerHtml.AppendHtml(icon);

            output.Content.AppendHtml(container);
        }
    }
}
