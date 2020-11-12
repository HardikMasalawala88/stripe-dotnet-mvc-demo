using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StripeGateway.Controllers
{
    public class StripeController : Controller
    {
        public StripeController()
        {

            Stripe.StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["stripePublishableKey"]);
            Stripe.StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["stripeSecretkey"];
        }
        // GET: Stripe
        public ActionResult Index()
        {
            ViewBag.StripePublishKey = ConfigurationManager.AppSettings["stripePublishableKey"];
            var planList = StripePlanList();
            return View(planList);
        }

        [HttpPost]
        public ActionResult Charge(string stripeToken, string stripeEmail,string stripeTokenType)
        {

            var formdata = Request.Form;
            var customer = createStripeCustomer(stripeToken, stripeEmail);
            var subscription = CreateSubscription(customer, "1000");
            //var chargeOptions = new ChargeCreateOptions
            //{
            //    Customer = customer.Id,
            //    Description = "Custom t-shirt",
            //    Amount = 1000,
            //    Currency = "usd",
            //};
            //var chargeService = new ChargeService();
            //Charge charge = chargeService.Create(chargeOptions);

            #region MyRegion
            //if (Request.Form["stripeToken"] != null)
            //{
            //    var customers = new Stripe.CustomerService();
            //    var charges = new Stripe.ChargeService();

            //    var customer = customers.Create(new Stripe.CustomerCreateOptions
            //    {
            //        Email = Request.Form["stripeEmail"],
            //        //SourceToken = Request.Form["stripeToken"]

            //    });

            //    //var charge = charges.Create(new Stripe.ChargeCreateOptions
            //    //{
            //    //    Amount = Convert.ToInt32(Request.Form["amount"]) * 100,
            //    //    Description = "Sample Charge",
            //    //    Currency = "inr",
            //    //    Customer = customer.Id,
            //    //    Shipping = new Stripe.ShippingOptions {
            //    //        Name = "Shubham",
            //    //        Phone = "232323",
            //    //        Address = new AddressOptions {
            //    //            City = "Ku",
            //    //            Country = "India",
            //    //            Line1 = "2637",
            //    //            PostalCode = "132103",
            //    //            State = "Haryana"
            //    //        }

            //    //    }

            //    //});

            //}
            #endregion
            //var customers = new StripeCustomerService();
            //var charges = new StripeChargeService();
            //var subscriptions = new StripeSubscriptionService();

            //var customer = customers.Create(new StripeCustomerCreateOptions
            //{
            //    Email = stripeEmail,
            //    SourceToken = stripeToken
            //});

            //var charge = charges.Create(new StripeChargeCreateOptions
            //{
            //    Amount = 500,
            //    Description = "Sample Charge",
            //    Currency = "usd",
            //    CustomerId = customer.Id
            //});

            //var subscription = subscriptions.Create(customer.id, new StripeSubscriptionCreateOptions()
            //{
            //    PlanId = "your-plan-here"
            //};

            //#region Old code

            //try
            //{
            //    var formData = Request;
            //    var myCharge = new Stripe.ChargeCreateOptions();
            //    // always set these properties
            //    myCharge.Amount = 500;
            //    myCharge.Currency = "USD";
            //    myCharge.ReceiptEmail = stripeEmail;
            //    myCharge.Description = "Sample Charge";
            //    myCharge.Source = stripeToken;
            //    myCharge.Capture = true;
            //    var chargeService = new Stripe.ChargeService();
            //    Charge stripeCharge = chargeService.Create(myCharge);
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
            //#endregion
            return View();
        }

        public List<Plan> StripePlanList()
        {
            var options = new PlanListOptions { Limit = 10 };
            var service = new PlanService();
            //StripeList<Plan> plans = service.List(options);
            var plans = service.List(options).Data;
            return plans;
        }
        #region StripeCustomer
        public Customer createStripeCustomer(string stripeToken,string stripeEmail)
        {
            try
            {
                string stripeCustId = string.Empty;
                //fetch stripeCustId from DB using email if found then 
                //demo customer ID : "cus_IN8KSF6jWoAF4U"
                //var service = new CustomerService();
                //service.Get("cus_IN8KSF6jWoAF4U");

                Customer customer  = new Customer();
                if (string.IsNullOrEmpty(stripeCustId))
                {
                    //Create customer on stripe
                    var customerOptions = new CustomerCreateOptions
                    {
                        Email = stripeEmail,
                        Source = stripeToken,
                    };
                    var customerService = new CustomerService();
                    customer = customerService.Create(customerOptions);
                }
                else {
                    //Fetch customer from stripe
                    var service = new CustomerService();
                    customer = service.Get(stripeCustId);
                }
                return customer;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        #endregion
        #region subscription
        
        //public Subscription CreateSubscription([FromBody] CreateSubscriptionRequest req)
        public Subscription CreateSubscription(Customer customer,string price)
        {
            //// Attach payment method
            //var options = new PaymentMethodAttachOptions
            //{
            //    Customer = req.Customer,
            //};
            //var service = new PaymentMethodService();
            //var paymentMethod = service.Attach(req.PaymentMethod, options);

            //// Update customer's default invoice payment method
            //var customerOptions = new CustomerUpdateOptions
            //{
            //    InvoiceSettings = new CustomerInvoiceSettingsOptions
            //    {
            //        DefaultPaymentMethod = paymentMethod.Id,
            //    },
            //};
            //var customerService = new CustomerService();
            //customerService.Update(req.Customer, customerOptions);

            // Create subscription
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = customer.Id,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = Environment.GetEnvironmentVariable(price),
                    },
                },
            };
            subscriptionOptions.AddExpand("latest_invoice.payment_intent");
            var subscriptionService = new SubscriptionService();
            try
            {
                Subscription subscription = subscriptionService.Create(subscriptionOptions);
                return subscription;
            }
            catch (StripeException e)
            {
                Console.WriteLine($"Failed to create subscription.{e}");
                return null;
            }
        }
        #endregion

    }
}