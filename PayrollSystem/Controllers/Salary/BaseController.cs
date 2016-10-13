using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayrollSystemLibrary;

namespace PayrollSystem.Controllers.Salary
{
    public class BaseController : Controller
    {
        public PayrollSystemEntities db = new PayrollSystemEntities();

        public Employee emp = new Employee();
        public ActionResult Logout()
        {
            Session.Remove("user");
            return RedirectToAction("../Salary/");
        }
        public ActionResult LoggedInNavigation()
        {
            return PartialView();
        }

        public string ViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}