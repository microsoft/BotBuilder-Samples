namespace TestBot.Controllers
{
    using System;
    using System.Web.Configuration;
    using System.Web.Mvc;

    [RoutePrefix("Chat")]
    public class ChatController : Controller
    {
        // GET: Chat
        public ActionResult Index(string commands)
        {
            this.ViewBag.UserId = Guid.NewGuid().ToString();
            this.ViewBag.DirectLineSecret = WebConfigurationManager.AppSettings["DirectLineSecret"];
            this.ViewBag.Commands = commands;

            return this.View();
        }

        // GET: Chat
        [Route("TestContainer")]
        public ActionResult TestContainer()
        {
            return this.View();
        }
    }
}