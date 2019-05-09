using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using System.Collections.Generic;

namespace SmartBreadcrumbs.Nodes
{
    public abstract class BreadcrumbNode
    {

        #region Properties

        public string Title { get; set; }

        public string OriginalTitle { get; }

        public Dictionary<string, object> RouteValues { get; set; }

        public IEnumerable<string> RouteValueKeys { get; set; }

        public bool OverwriteTitleOnExactMatch { get; set; }

        public string IconClasses { get; set; }

        public BreadcrumbNode Parent { get; set; }

        #endregion

        internal BreadcrumbNode(BreadcrumbAttribute attr) :
            this(attr.Title, attr.OverwriteTitleOnExactMatch, attr.IconClasses, attr.AreaName, attr.RouteValueKeys)
        {

        }

        protected BreadcrumbNode(string title, bool overwriteTitleOnExactMatch = false, string iconClasses = null, string areaName = null, IEnumerable<string> routeValueKeys = null)
        {
            Title = title;
            OriginalTitle = Title;
            OverwriteTitleOnExactMatch = overwriteTitleOnExactMatch;
            IconClasses = iconClasses;

            if (!string.IsNullOrWhiteSpace(areaName))
            {
                RouteValues = new Dictionary<string, object>() { { "area", areaName } };
            }

            RouteValueKeys = routeValueKeys;
        }

        #region Public Methods

        public abstract string GetUrl(IUrlHelper urlHelper);

        public abstract string GetUrl(IUrlHelper urlHelper, Dictionary<string, object> overriddenRouteValues);
        #endregion

    }
}
