using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Integration.AspNet.Core.Handlers;
using Newtonsoft.Json;
using TeamsTasksBot.Models;

namespace TeamsTasksBot.Controllers
{
    public class HomeController : Controller
    {

        [Route("helloworld")]
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("configure")]
        public ActionResult Configure()
        {
            return View();
        }

        [Route("tasks")]
        public ActionResult Tasks()
        {
            return View();
        }

        [Route("customform")]
        [HttpGet]
        public async Task<ActionResult> CustomForm()
        {
            return View();
        }

        [Route("customform")]
        [HttpPost]
        public async Task<ActionResult> CustomForm([FromForm] string name, string email, string favoriteBook, string password, string confirmPassword)
        {
            return Content($"Thanks. \n\nName: {name}\n\nEmail: {email}\n\nFavorite Book: {favoriteBook} \n\n(Password omitted)");
        }

        [Route("youtube")]
        public ActionResult YouTube()
        {
            return View();
        }

        [Route("powerapp")]
        public ActionResult PowerApp()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
