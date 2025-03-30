using ProjectPRNLamnthe180410.Models.ViewModel;

namespace ProjectPRNLamnthe180410.Services.Interface
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentResponseModel model, HttpContext context);

        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
