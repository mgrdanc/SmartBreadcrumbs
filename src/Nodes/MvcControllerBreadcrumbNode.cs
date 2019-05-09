using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SmartBreadcrumbs.Attributes;
using System.Collections.Generic;

namespace SmartBreadcrumbs.Nodes
{
    public class MvcControllerBreadcrumbNode : BreadcrumbNode
    {
        #region Properties
        public string Controller { get; set; }

        #endregion

        internal MvcControllerBreadcrumbNode(string controller, BreadcrumbAttribute attr) : base(attr)
        {
            Controller = controller;
        }

        public MvcControllerBreadcrumbNode(string controller, string title, bool overwriteTitleOnExactMatch = false, string iconClasses = null, string areaName = null, IEnumerable<string> routeValueKeys = null)
            : base(title, overwriteTitleOnExactMatch, iconClasses, areaName, routeValueKeys)
        {
            Controller = controller;
        }

        #region Public Methods
        public override string GetUrl(IUrlHelper urlHelper) => urlHelper.Action(new UrlActionContext() { Action = BreadcrumbManager.Options.DefaultAction, Controller = Controller, Values = RouteValues });

        public override string GetUrl(IUrlHelper urlHelper, Dictionary<string, object> overriddenRouteValues) => urlHelper.Action(new UrlActionContext() { Action = BreadcrumbManager.Options.DefaultAction, Controller = Controller, Values = overriddenRouteValues });
        #endregion
    }
}
