using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using WarehouseManager.Models.ViewModels;

namespace WarehouseManager.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "paging-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory urlHelperFactory;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public ListViewModel PagingModel { get; set; }
        public string PageAction { get; set; }
        public string PageController { get; set; }

        // Styles properties
        public bool PageClassesEnabled { get; set; }
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected  { get; set; }

        public PageLinkTagHelper(IUrlHelperFactory helper) => urlHelperFactory = helper;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");

            for (int i = 1; i <= PagingModel.PagingInfo.TotalPages; i++)
            {
                TagBuilder tag = new TagBuilder("a");

                tag.Attributes["href"] = urlHelper.Action(PageAction, PageController,
                    new { page = (i == 1) ? "" : i.ToString(),
                        orderby = (PagingModel.ListFilter.OrderBy == "id") ? "" : PagingModel.ListFilter.OrderBy,
                        order = (PagingModel.ListFilter.Order == "asc") ? "" : PagingModel.ListFilter.Order,
                        searchby = PagingModel.ListFilter.SearchBy as string,
                        search = PagingModel.ListFilter.Search,
                        initdate = PagingModel.ListFilter.InitDate,
                        enddate = PagingModel.ListFilter.EndDate
                    });
                tag.InnerHtml.Append(i.ToString());

                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PagingModel.PagingInfo.Page ? PageClassSelected : PageClassNormal);
                }

                result.InnerHtml.AppendHtml(tag);
            }

            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}