namespace ContosoFlowers.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Microsoft.Bot.Builder.ConnectorEx;
    using Microsoft.Bot.Builder.Dialogs;
    using Services;
    using Services.Models;

    [RoutePrefix("CheckOut")]
    [RequireHttps]
    public class CheckOutController : Controller
    {
        private readonly IOrdersService ordersService;

        public CheckOutController(IOrdersService ordersService)
        {
            this.ordersService = ordersService;
        }

        [Route("")]
        [HttpGet]
        public ActionResult Index(string state, string orderId)
        {
            var order = this.ordersService.RetrieveOrder(orderId);

            // Check order exists
            if (order == null)
            {
                throw new ArgumentException("Order Id not found", "orderId");
            }

            // Check order if order is already processed
            if (order.Payed)
            {
                return this.RedirectToAction("Completed", new { orderId = orderId });
            }

            // Payment form
            this.ViewBag.State = state;
            return this.View(order);
        }

        [Route("")]
        [HttpPost]
        public async Task<ActionResult> Index(
            string botId,
            string channelId,
            string conversationId,
            string serviceUrl,
            string userId,
            string orderId,
            PaymentDetails paymentDetails)

        {
            this.ordersService.ConfirmOrder(orderId, paymentDetails);

            var address = new Address(botId, channelId, userId, conversationId, serviceUrl);
            var conversationReference = address.ToConversationReference();
            var message = conversationReference.GetPostToBotMessage();

            message.Text = orderId;

            await Conversation.ResumeAsync(conversationReference, message);

            return this.RedirectToAction("Completed", new { orderId = orderId });
        }

        [Route("completed")]
        public ActionResult Completed(string orderId)
        {
            var order = this.ordersService.RetrieveOrder(orderId);
            if (order == null)
            {
                throw new ArgumentException("Order Id not found", "orderId");
            }

            return this.View("Completed", order);
        }
    }
}