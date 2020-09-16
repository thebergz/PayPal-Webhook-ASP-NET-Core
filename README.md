# README #

![Sandbox PayPal Payment](./WebHook.png)

#### This README covers the basics of this PayPal Webhook API sample project by Bergzsoft. ####


## PayPal Webhook WebAPI v1.0 ##

### The project is based on: ###

* ASP.NET CORE API 3.1  
* .NET Core 3.1  
* PayPal SDK  

[Learn Markdown](https://bitbucket.org/tutorials/markdowndemo)

# How to get this running #

## Clone the repo ##

* Install **[ git ](https://git-scm.com/downloads)** if you don't already have it.
* Clone this repo (open a command prompt in the folder where you want the repo root folder created, then type *git clone <repo-url>* which you get from the repo clone link)

## Setup Visual Studio ##

* Install **[ Visual Studio 2019 ](https://visualstudio.microsoft.com/downloads/)** - Community Edition is FREE
* Configuration - Default is fine
* Dependencies: (these maybe installed/pulled automagically - compile then check NuGet Package Manager and grab the below if they aren't already there)
  * NuGet Install PayPal SDK v1.9.1
  * NuGet Install System.Configuration.ConfigurationManager


## Test Dev Environment Setup ##

Allow external (eg.*PayPal*) calls into a local IISExpress (ie. the default Visual Studio dev) environment.

* Get the **[ Keyoti Conveyor ](https://marketplace.visualstudio.com/items?itemName=vs-publisher-1448185.ConveyorbyKeyoti)** plugin for Visual Studio then  
  Either:
    * **Forward TCP port 443** at your internet router INTO the port used by Keyoti (details later - it can wait for now)
        - This may require occasional updates at your router as the port does change in the free version
    * Use the Keyoti **Internet Access** option
        - This may require a custom URL in PayPal and **HAS NOT BEEN TESTED** by the author here ;)

## PayPal Sandbox Environment Setup ##

* Login to PayPal
* Goto the Developer Dashboard
* Create a **sandbox** account
    * You should end up with a 	Facilitator (web side) and buyer (purchaser side) account
* Open MyApps & Credentials
    * Create a **sandbox** App
        * Link the **sandbox** account (faciitator) created earlier
        * Scroll down and "Add Webhook"
            * Enter the URL to your host - usually a dynamic DNS address for your development site (OR the URL provided by your port forwarding internet proxy)
            * Select the notification events of interest. I recommend ***Payment Capture Completed*** as a starting point for testing

## Update appsettings.json in the Code ##

* In PayPal Developer Dashboard, get the following from the business/facilitator account:
    * Client ID
    * ClientSecret
    - Then copy these into the relevant fields in appsettings.json

* Get your PushOver App and User API keys and update the relevant fields in appsettings.json

# Time to Compile #

* Debug (F5) the app.
* Once running, click Tools/Keyoti Conveyor

## Setup your firewall port forwarding ##

* Get the port that [Keyoti](https://marketplace.visualstudio.com/items?itemName=vs-publisher-1448185.ConveyorbyKeyoti) is using for external Access
* Add port forward rule:  
    443 external -> local machine IP : Keyoti **external** port

## Use Postman to do the initial access test ##

* Install the **[ Postman App ](https://www.postman.com/downloads/)**
* Login to Postman (register first if required)
* Create a new test
    * Name it something like WebHook Access Test
    * Set the method to POST
    * In the URL put your external URL + the path to the API which is /api/Webhook
        - eg. https://myexternal.ip.com/api/Webhook
    * Select Body
    * Select Raw
    * Copy the contents of POST-TEST.json (in the Resources folder) into the Body of the Postman test
    * SAVE the Postman test
    * **RUN**
    * Check the result - stick a breakpoint in VS to see where it got if there's any uncertainty

## Test Webhook calls using the PayPal Simulator ##

* Login to PayPal Developer Dashboard
* Goto MOCK / Webhook Simulator
* Enter the Webhook URL, Event (Eg. Payment Capture Completed, and version Eg. 2)
* Test the webhook
**NOTE:** If all is well you may already be receiving Push Messages. If not.... breakpoint time.
    Also Note, the Webhook Simulator does not accept callbacks which are needed in the code, so as long as the Webhook simulator gets TO your API you can move onto the PayPal Payment test.

# Testing PayPal Payments #

* Clone and Run the PayPal Payment Test App
* Use the PayPal Pay button in the ***Payment*** menu
* Login with your PayPal Sandbox **buyer** account and complete the transaction
    * With a bit of luck you should shortly receive full blown PayPal webhook notifications with extracted sample information.

# Moving into Production #
Note: This assumes Windows 2012R2 or higher and IIS **x64** worker process

## PayPal Updates ##
1. Create your production PayPal App like you did in the Sandbox and take note of the ClientID and Client Secret that you associated
2. Configure the Webhook URL to be your IIS Servers external URL, and enable the required events

## Visual Studio ##
1. **Update** the PayPal ***clientID*** and ***clientSecret*** entries to use the PayPal PRODUCTION values, in **appSettings.json**  
    **NOTE:** - DO NOT PUSH THESE KEYS TO GIT - KEEP THEM SAFE!
2. You may want to remove the WeatherForecast Controller, although it is quite benign and handy for testing.  
    **NOTE:** If you are leaving this in, you should enable AUTHENTICATION and this requires appropriate attributes and an Authentication mechanism which we don't have in this project. I suggest removing it either immediately or after testing (and then republish).
3. Run the `publish.bat` file

## IIS Setup ##
1. Copy the publishpaypalwebhook folder to your web server
2. On the IIS Server ensure the following are installed [(available here)](https://dotnet.microsoft.com/download/dotnet-core/3.1):
	* .NET Core Runtime x64
	* ASP.NET Core Runtime x64
	* ASP.NET Core Runtime **Hosting Bundle** x64
3. Add a new Application Pool [(as shown here)](./AddAppPool.png)  
    **NOTE:** **Check the *Advanced Settings* for the Application Pool** and make sure the **"Enable 32-bit Applications"** settings matches your deployment profile. In the case of this example, that means x64 which means the settings should be **false** (to force a 64 bit Worker Process and avoid errors).
4. Under an existing Site, Add a new Application [(as shown here)](./AddApplication.png)
5. Add any firewall rules to enable access

## External URL Access Verification ##

The first thing to know is the URL to your app. If the App Alias on your webserver was "paypalwebhook" and your URL is "myserver.dyndns.net" then the path to the webhook will be:
    **https://myserver.dyndns.net/paypalwebhook/Webhook**  

1. Test external access to your page by either:
    * Using Postman - **NOTE: Because the Webhook calls back to PayPal it will likely fail but don't worry, even a 404 Not Found means *access* IS working.**  
    **OR**  
    * By accessing the https://YourWebhookURL/WeatherForecast URL (if you have not removed it yet)

## Verification Testing ##
1. Use the PayPal Payment Test App to pay yourself $0.01 (1 cent) using a LIVE PayPal account
2. Check that you get a push notification.
3. You are done

## ENJOY :) ##

# Other Stuff #

## Troubleshooting ##

* Check that your inbound listener port has not changed and that your gateway is natting correctly from 443->external listener port
* Use Postman for basic access verification
* DO NOT use a PORT in the URL you provide to PayPal. External listener MUST BE on 443
* Check your Push Notification API keys

## Contribution guidelines ##

* Writing tests
* Code review
* Updating the ReadMe - try [Visual Studio Code](https://code.visualstudio.com/download)

## Who do I talk to? ##

* Repo owner or admin - thebergz@gmail.com
* Other community or team contact - google

## Handy References ##
* A way to [**port forward external** access **to local IIS**](https://programmingflow.com/2017/02/25/iis-express-on-external-ip.html) - 
    * This worked briefly then didn't so I stumbled across and switched to the [Keyoti Conveyor](https://marketplace.visualstudio.com/items?itemName=vs-publisher-1448185.ConveyorbyKeyoti) described earlier

* [ Postman self signed certs ](https://blog.postman.com/using-self-signed-certificates-with-postman/)

* I think I watched [the first 5 minutes of this](https://www.youtube.com/watch?v=1ck9LIBxO14&feature=emb_rel_end) to learn about .Net CORE WebAPI - seemed interesting 