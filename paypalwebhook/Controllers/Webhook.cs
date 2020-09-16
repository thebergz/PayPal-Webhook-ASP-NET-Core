using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using PayPal.Api;
using paypalwebhook.Services;

namespace paypalwebhook.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Webhook : ControllerBase
    {
        private IConfiguration config;

        public Webhook(IConfiguration iconfig)
        {
            this.config = iconfig;
        }

        // POST: Webhook
        [HttpPost]
        public async Task<ActionResult> PostPaymentAsync()
        {
            // The APIContext object can contain an optional override for the trusted certificate.
            //var payPalContext = PayPalHandler()

            var apiContext = PayPalConfiguration.GetAPIContext(config);

            // Get the received request's headers
            var requestheaders = HttpContext.Request.Headers;

            // Get the received request's body
            var requestBody = string.Empty;
            using (var reader = new System.IO.StreamReader(HttpContext.Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            dynamic jsonBody = JObject.Parse(requestBody);
            string webhookId = jsonBody.id;

            string event_type = jsonBody.event_type;
            string create_time = jsonBody.create_time;

            // We have all the information the SDK needs, so perform the validation.
            // Note: at least on Sandbox environment this returns false.
            // var isValid = WebhookEvent.ValidateReceivedEvent(apiContext, ToNameValueCollection(requestheaders), requestBody, webhookId);
            var ev = new WebhookEvent();
            try
            {
                ev = WebhookEvent.Get(apiContext, webhookId);
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                throw;
            }
            
            switch (ev.event_type)
            {
                case "PAYMENT.CAPTURE.COMPLETED":
                    // Handle payment completed
                    break;
                case "PAYMENT.CAPTURE.DENIED":
                    // Handle payment denied
                    break;
                // Handle other webhooks
                default:
                    break;
            }

            Newtonsoft.Json.Linq.JObject resJson = (Newtonsoft.Json.Linq.JObject)ev.resource;

            String amount = new String("");
            try
            {
                amount = (string)(Newtonsoft.Json.Linq.JValue)resJson["amount"]["value"]; //ev.resource.amount.value.ToString;  //resource_body.amount.value;
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                throw;
            }
            

            var parameters = new NameValueCollection {
                    { "token", config.GetSection("PushNotifications:appToken").Value }, // Your PushOver APP Token goes here
                    { "user", config.GetSection("PushNotifications:userToken").Value },  // Your PushOver USER Token goes here
                    { "message", event_type + " received at " + create_time + " with ID: " + webhookId + " for amount: " + amount }
            };

            using (var client = new WebClient())
            {
                client.UploadValues(config.GetSection("PushNotifications:endpoint").Value, parameters);
            }

            return Ok("Ok");
        }
    }
}
