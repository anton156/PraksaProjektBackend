using PraksaProjektBackend.Auth;
using Stripe;

namespace PraksaProjektBackend.Services
{
    public class MakePayment
    {
        public static async Task<dynamic> PayAsync(string cardnumber, int month, int year , string cvc, int value)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51KfnsTGklHgcWdIW5HnZ94npevVpnlFKrRqa8UintmjlWZqeOn7p75NvD3f0m8qI4y8fqDTxxH2HZj8zGrqRUadR00ow9Z9QQQ";

                var optionstoken = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = cardnumber,
                        ExpMonth = month,
                        ExpYear = year,
                        Cvc = cvc,

                    }
                };

                var servicetoken = new TokenService();
                Token stripetoken = await servicetoken.CreateAsync(optionstoken);

                var options = new ChargeCreateOptions
                {
                    Amount = value,
                    Currency = "usd",
                    Description = "test",
                    Source = stripetoken.Id
                };

                var service = new ChargeService();
                Charge charge = await service.CreateAsync(options);
                if (charge.Paid)
                {
                    return charge.Id;
                }
                else
                {
                    return "Failed";
                }

            }
            catch (Exception ex)
            {
                return "Failed " + ex.Message;
            }
        }
        public static async Task<StripeList<Charge>> GetCharges()
        {

            StripeConfiguration.ApiKey = "sk_test_51KfnsTGklHgcWdIW5HnZ94npevVpnlFKrRqa8UintmjlWZqeOn7p75NvD3f0m8qI4y8fqDTxxH2HZj8zGrqRUadR00ow9Z9QQQ";

            var options = new ChargeListOptions { Limit = 3 };
            var service = new ChargeService();
            StripeList<Charge> charges = service.List(
              options);

            return charges;
        }

        public static async Task<Charge> GetOneCharge(string id)
        {

            StripeConfiguration.ApiKey = "sk_test_51KfnsTGklHgcWdIW5HnZ94npevVpnlFKrRqa8UintmjlWZqeOn7p75NvD3f0m8qI4y8fqDTxxH2HZj8zGrqRUadR00ow9Z9QQQ";

            var service = new ChargeService();
            var charge = service.Get(id);

            return charge;
        }

        public static async Task<dynamic> Refund(string id)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51KfnsTGklHgcWdIW5HnZ94npevVpnlFKrRqa8UintmjlWZqeOn7p75NvD3f0m8qI4y8fqDTxxH2HZj8zGrqRUadR00ow9Z9QQQ";

                var options = new RefundCreateOptions
                {
                    Charge = id,
                };
                var service = new RefundService();
                Refund refund = await service.CreateAsync(options);
                if (refund.Object == "refund")
                {
                    return "Success";
                }
                else
                {
                    return "Failed";
                }
            }
            catch (Exception ex)
            {
                return "Failed " + ex.Message;
            }
        }

        public static async Task<StripeList<Refund>> GetAllRefunds()
        {

            StripeConfiguration.ApiKey = "sk_test_51KfnsTGklHgcWdIW5HnZ94npevVpnlFKrRqa8UintmjlWZqeOn7p75NvD3f0m8qI4y8fqDTxxH2HZj8zGrqRUadR00ow9Z9QQQ";

            var options = new RefundListOptions { Limit = 3 };
            var service = new RefundService();
            StripeList<Refund> refunds = service.List(
              options);


            return refunds;
        }

        public static async Task<Refund> GetOneRefund(string id)
        {

            StripeConfiguration.ApiKey = "sk_test_51KfnsTGklHgcWdIW5HnZ94npevVpnlFKrRqa8UintmjlWZqeOn7p75NvD3f0m8qI4y8fqDTxxH2HZj8zGrqRUadR00ow9Z9QQQ";

            var service = new RefundService();
            var refund = service.Get(id);

            return refund;
        }
    }
}
