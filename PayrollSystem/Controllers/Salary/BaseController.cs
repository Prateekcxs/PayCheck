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
    }
}