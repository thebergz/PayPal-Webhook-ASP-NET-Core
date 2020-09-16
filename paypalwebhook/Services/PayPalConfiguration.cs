using System.Collections.Generic;
using PayPal.Api;
using Microsoft.Extensions.Configuration;

namespace paypalwebhook.Services
{
    public class PayPalConfiguration
    {
        public static PayPal.Api.APIContext GetAPIContext(IConfiguration config)
        {

            //@ Declare a R/O property to store our own custom configuration for PayPal
             Dictionary<string, string> _payPalConfig;

            //@ Fetch the `appsettings.json` and pack it into the custom configuration
            //@ Only the *clientId*,*clientSecret* and *mode* are required to get an access token (as of the time of writing this)
            _payPalConfig = new Dictionary<string, string>()
            {
                { "clientId" , config.GetSection("PayPal:clientId").Value },
                { "clientSecret", config.GetSection("PayPal:clientSecret").Value },
                { "mode", config.GetSection("PayPal:mode").Value },
                //{ "business", config.GetSection("paypal:business").Value },
                //{ "merchantId", config.GetSection("paypal:merchantId").Value },
            };


            // Authenticate with PayPal
            //var config = ConfigManager.Instance.GetProperties();
            //var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var accessToken = new OAuthTokenCredential(_payPalConfig).GetAccessToken();
            var apiContext = new APIContext(accessToken);
            return apiContext;
        }

    }

    
}
