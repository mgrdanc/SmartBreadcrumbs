using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using System.Collections.Generic;

namespace SmartBreadcrumbs.Nodes
{
    public class MvcBreadcrumbNode : BreadcrumbNode
    {

        #region Properties

        public string Action { get; set; }

        public string Controller { get; set; }

        #endregion

        internal MvcBreadcrumbNode(string action, string controller, BreadcrumbAttribute attr) : base(attr)
        {
            Action = action;
            Controller = controller;
        }

        public MvcBreadcrumbNode(string action, string controller, string title, bool overwriteTitleOnExactMatch = false, string iconClasses = null, string areaName = null, IEnumerable<string> routeValueKeys = null)
            : base(title, overwriteTitleOnExactMatch, iconClasses, areaName, routeValueKeys)
        {
            Action = action;
            Controller = controller;
        }

        #region Public Methods

        public override string GetUrl(IUrlHelper urlHelper) => urlHelper.Action(Action, Controller, RouteValues);

        public override string GetUrl(IUrlHelper urlHelper, Dictionary<string, object> overriddenRouteValues) => urlHelper.Action(Action, Controller, overriddenRouteValues);
        #endregion

    }
}
