using Microsoft.Bot.Builder.Dialogs;
using PayPal.Api;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bot.Controllers
{
    public class PaymentController : ApiController
    {
        [HttpGet]
        // POST api/<controller>
        [Route("api/Payment")]
        //Example: http://localhost:3979/api/Payment?resume=H4sIAAAAAAAEAEWOUQ6CMAyGmz0YPYHHYCAicAHjiy_KAcqogWSsZOs4nmdzRhLf_n79mv5vAFAxkL8NcEi5MLm5vPD0g3ecCXYJd2nIQZkRnSO7uTRHi8IeVM-ysXNVa30qSlDpYp0Mdd7CMS1GkaXNMssG7chB2kZrncF-Ck8fg9Dw-PtfevUcl9TNsFvJB5SJ3fajxqouh74B-ADTzn_4vgAAAA2&cancel=true&token=EC-01U772690T6522532
        public async Task<HttpResponseMessage> Payment([FromUri] string resume, [FromUri] string token, [FromUri] string paymentId = null, [FromUri] string PayerID = null, [FromUri] bool cancel = false)
        {
            //Get resumption cookie
            var resumptionCookie = UrlToken.Decode<ResumptionCookie>(resume);
            var msg = resumptionCookie.GetMessage();

            if (cancel)
            {
                msg.Text = "You have canceled your payment, please try again.";
            }
            else
            {
                var paymentExecution = new PaymentExecution() { payer_id = PayerID };
                var payment = new Payment() { id = paymentId };

                // Execute the payment
                var executedPayment = payment.Execute(Bot.Utilities.Configuration.GetAPIContext(), paymentExecution);
                
                if (executedPayment.state.ToLower() == "approved")
                {
                    //GetMessage 
                    msg.Text = "Thanks!";
                }
                else
                {
                    msg.Text = "I am sorry your payment was not successful, please try again.";
                }
            }
            await Conversation.ResumeAsync(resumptionCookie, msg);
            HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(string.Concat(msg.Text, " Please return to the conversation with the bot to continue..."));
            return resp;
        }


    }
}