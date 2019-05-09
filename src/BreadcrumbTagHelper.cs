using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SmartBreadcrumbs.Nodes;

namespace SmartBreadcrumbs
{
    [HtmlTargetElement("breadcrumb")]
    public class BreadcrumbTagHelper : TagHelper
    {

        #region Fields

        private readonly BreadcrumbManager _breadcrumbManager;
        private readonly IUrlHelper _urlHelper;

        #endregion

        #region Properties

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        #endregion

        public BreadcrumbTagHelper(BreadcrumbManager breadcrumbManager, IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
        {
            _breadcrumbManager = breadcrumbManager;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        #region Public Methods

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var child = await output.GetChildContentAsync();

            string nodeKey = GetNodeKey(ViewContext.ActionDescriptor.RouteValues);
            var node = ViewContext.ViewData["BreadcrumbNode"] as BreadcrumbNode ?? _breadcrumbManager.GetNode(nodeKey);

            output.TagName = BreadcrumbManager.Options.TagName;

            // Tag Classes
            if (!string.IsNullOrEmpty(BreadcrumbManager.Options.TagClasses))
            {
                output.Attributes.Add("class", BreadcrumbManager.Options.TagClasses);
            }

            output.Content.AppendHtml($"<ol class=\"{BreadcrumbManager.Options.OlClasses}\">");

            var sb = new StringBuilder();

            // Go down the hierarchy
            if (node != null)
            {
                if (node.OverwriteTitleOnExactMatch && node.Title.StartsWith("ViewData."))
                    node.Title = ExtractTitle(node.OriginalTitle);

                sb.Insert(0, GetLi(node, node.GetUrl(_urlHelper), true));

                while (node.Parent != null)
                {
                    node = node.Parent;

                    // Separator
                    if (BreadcrumbManager.Options.HasSeparatorElement)
                    {
                        sb.Insert(0, BreadcrumbManager.Options.SeparatorElement);
                    }

                    //Extract if there are more RouteValues needed
                    if (node.RouteValueKeys != null)
                    {                        
                        sb.Insert(0, GetLi(node, node.GetUrl(_urlHelper, ExtractRouteKeyValue(node.RouteValues, node.RouteValueKeys)), false));
                    }
                    else
                    {
                        sb.Insert(0, GetLi(node, node.GetUrl(_urlHelper), false));
                    }
                }
            }

            // If the node was custom and it had no defaultnode
            if (node != _breadcrumbManager.DefaultNode)
            {
                // Separator
                if (BreadcrumbManager.Options.HasSeparatorElement)
                {
                    sb.Insert(0, BreadcrumbManager.Options.SeparatorElement);
                }

                sb.Insert(0, GetLi(_breadcrumbManager.DefaultNode,
                    _breadcrumbManager.DefaultNode.GetUrl(_urlHelper),
                    false));
            }

            output.Content.AppendHtml(sb.ToString());
            output.Content.AppendHtml(child);
            output.Content.AppendHtml("</ol>");
        }

        #endregion

        #region Private Methods

        private string GetNodeKey(IDictionary<string, string> routeValues)
        {
            if (routeValues.ContainsKey("page") && !string.IsNullOrWhiteSpace(routeValues["page"]))
                return routeValues["page"];
            else if (routeValues.ContainsKey("controller") && !routeValues.ContainsKey("action"))
                return $"{routeValues["controller"]}";

            if (!HttpMethods.IsGet(ViewContext.HttpContext.Request.Method))
            {
                return $"{routeValues["controller"]}.{routeValues["action"]}#{ViewContext.HttpContext.Request.Method}";
            }
            return $"{routeValues["controller"]}.{routeValues["action"]}";
        }

        private string ExtractTitle(string title)
        {
            if (!title.StartsWith("ViewData."))
                return title;

            string key = title.Substring(9);
            return ViewContext.ViewData.ContainsKey(key) ? ViewContext.ViewData[key].ToString() : $"{key} Not Found";
        }

        private static string GetClass(string classes)
        {
            return string.IsNullOrEmpty(classes) ? "" : $" class=\"{classes}\"";
        }

        private string GetLi(BreadcrumbNode node, string link, bool isActive)
        {
            // In case the node's title is still ViewData.Something
            string nodeTitle = ExtractTitle(node.Title);

            var normalTemplate = BreadcrumbManager.Options.LiTemplate;
            var activeTemplate = BreadcrumbManager.Options.ActiveLiTemplate;

            if (!isActive && string.IsNullOrEmpty(normalTemplate))
                return $"<li{GetClass(BreadcrumbManager.Options.LiClasses)}><a href=\"{link}\">{nodeTitle}</a></li>";

            if (isActive && string.IsNullOrEmpty(activeTemplate))
                return $"<li{GetClass(BreadcrumbManager.Options.LiClasses)}>{nodeTitle}</li>";

            // Templates
            string templateToUse = isActive ? activeTemplate : normalTemplate;

            // The IconClasses will get ignored if the template doesn't have their index.
            return string.Format(templateToUse, nodeTitle, link, node.IconClasses);
        }

        private Dictionary<string, object> ExtractRouteKeyValue(Dictionary<string, object> routeValues, IEnumerable<string> keys)
        {
            //Reposity for object keyvalue pairs
            Dictionary<string, object> values = new Dictionary<string, object>(routeValues);

            //Loop through specified keys and search the ViewData for values
            foreach(string key in keys)
            {
                if (!ViewContext.ViewData.ContainsKey(key))
                    throw new SmartBreadcrumbsException($"Route Key {key} value was not found in ViewData.");

                //If the routevalues already contain the key and values we have do not add them again.
                if(!values.ContainsKey(key) )
                    values.Add(key, ViewContext.ViewData[key]);
            }

            return values;
        }

        #endregion

    }
}
