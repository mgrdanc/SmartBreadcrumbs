using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using System.Collections;

namespace RazorPagesAndMvc.Controllers.Java
{
    [Route("Java/Advanced/[action]")]
    public class AdvancedJavaController : Controller
    {
        [HttpGet]
        [Breadcrumb("Advanced", FromAction = "Index", FromController = typeof(JavaController))]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Breadcrumb("Advanced", FromAction = "Index", FromController = typeof(JavaController))]
        public IActionResult Index(int i)
        {
            return View();
        }

        [HttpGet]
        [Breadcrumb("Post 1", AreaName = "Stuff", RouteValueKeys = new string[] { "id" }, FromAction = "Index")]
        public IActionResult Post1(string id)
        {
            return View();
        }

        [HttpGet]
        [Breadcrumb("History", FromAction = "Post1", FromController = typeof(AdvancedJavaController))]
        public IActionResult Post1History()
        {
            //Add needed key for parent node
            ViewData.Add("id", "1");

            return View();
        }
    }
}