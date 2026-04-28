using api.application.Interface;
using Stripe;

namespace api.application.Services;

public class StripPaymentServices:IPaymentServices
{
    public async Task<bool> IsSuccessFullPayment(string id)
    {
        try
        {
            var paymentStrip = new PaymentIntentService();
            var paymentIntent = await paymentStrip.GetAsync(id);
            if (paymentIntent.Status == "succeeded")
                return true;
            return false;
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
       
    }
}