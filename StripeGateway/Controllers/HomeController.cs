using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StripeGateway.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            Stripe.StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["stripePublishableKey"]);
            Stripe.StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["stripeSecretkey"];
        }  
        public ActionResult Index()
        {
            ViewBag.StripePublishKey = ConfigurationManager.AppSettings["stripePublishableKey"];
            var List = StripePlanList();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Charge(string stripeToken, string stripeEmail)
        {
            #region Old code
            //Stripe.StripeConfiguration.SetApiKey("pk_test_51HlTFFClQJ3Lj3z37KoaDCHRDOn4E104spuyY6Qf7qi71HlRiTGgzdgO3yuonMs3rVge3RFDBF8ZAAtZRJSgwM5a00JwfKuuUm");
            //Stripe.StripeConfiguration.ApiKey = "sk_test_51HlTFFClQJ3Lj3z3qLfAQU3KTRWBJPQ8GvGvHWClYU67Eh0uKZlUKnUgm1gBF81d7FfHAS1WaXIDGf4yqgGsLIIW00pxaMXVmt";

            //var myCharge = new Stripe.ChargeCreateOptions();
            //// always set these properties
            //myCharge.Amount = 500;
            //myCharge.Currency = "USD";
            //myCharge.ReceiptEmail = stripeEmail;
            //myCharge.Description = "Sample Charge";
            //myCharge.Source = stripeToken;
            //myCharge.Capture = true;
            //var chargeService = new Stripe.ChargeService();
            //Charge stripeCharge = chargeService.Create(myCharge); 
            #endregion
            return View();
        }

        public List<Plan> StripePlanList()
        {
            var options = new PlanListOptions { Limit = 3 };
            var service = new PlanService();
            //StripeList<Plan> plans = service.List(options);
            var plans = service.List(options).Data;
            return plans;
        }
    }
}