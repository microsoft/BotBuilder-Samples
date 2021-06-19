using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamsMessagingExtensionsAction.Controllers
{
    public class HomeController : Controller
    {
        [Route("/Home/RazorView")]
        public ActionResult RazorView()
        {
            return View("RazorView");
        }

        [Route("/Home/CustomForm")]
        public ActionResult CustomForm()
        {
            return View("CustomForm");
        }
    }
}
