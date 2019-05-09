using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using System.Collections.Generic;

namespace SmartBreadcrumbs.Nodes
{
    public class RazorPageBreadcrumbNode : BreadcrumbNode
    {

        #region Properties

        public string Path { get; set; }

        #endregion

        internal RazorPageBreadcrumbNode(string path, BreadcrumbAttribute attr) : base(attr)
        {
            Path = path;
        }

        public RazorPageBreadcrumbNode(string path, string title, bool overwriteTitleOnExactMatch = false, string iconClasses = null, string areaName = null, IEnumerable<string> routeValueKeys = null)
            : base(title, overwriteTitleOnExactMatch, iconClasses, areaName, routeValueKeys)
        {
            Path = path;
        }

        #region Public Methods

        public override string GetUrl(IUrlHelper urlHelper) => urlHelper.Page(Path, RouteValues);
        
        public override string GetUrl(IUrlHelper urlHelper, Dictionary<string, object> overriddenRouteValues) => urlHelper.Page(Path, overriddenRouteValues);
        #endregion

    }
}
