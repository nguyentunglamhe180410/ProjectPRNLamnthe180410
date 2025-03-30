using ProjectPRNLamnthe180410.Services.Interface;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ProjectPRNLamnthe180410.Models.ViewModel;

namespace ProjectPRNLamnthe180410.Services
{



    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string CreatePaymentUrl(PaymentResponseModel model, HttpContext context)
        {
            string vnp_TmnCode = _configuration["Vnpay:TmnCode"];
            string vnp_HashSecret = _configuration["Vnpay:HashSecret"];
            string vnp_Url = _configuration["Vnpay:BaseUrl"];
            string vnp_ReturnUrl = _configuration["Vnpay:PaymentBackReturnUrl"];
            string vnp_CurrCode = _configuration["Vnpay:CurrCode"];
            string vnp_Command = _configuration["Vnpay:Command"];
            string vnp_Version = _configuration["Vnpay:Version"];
            string vnp_Locale = _configuration["Vnpay:Locale"];

            if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret) || string.IsNullOrEmpty(vnp_Url))
            {
                throw new Exception("Missing VNPAY configurations in appsettings.json.");
            }

            long transactionId = DateTime.Now.Ticks;
            long amount = (long)(model.Amount * 100000); // Convert to VND

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", vnp_Version);
            vnpay.AddRequestData("vnp_Command", vnp_Command);
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", amount.ToString());
            vnpay.AddRequestData("vnp_CurrCode", vnp_CurrCode);
            vnpay.AddRequestData("vnp_TxnRef", transactionId.ToString());
            vnpay.AddRequestData("vnp_OrderInfo", $"{model.Username} purchase {model.Amount} coins (ID: {transactionId})");
            vnpay.AddRequestData("vnp_OrderType", "VNPay");
            vnpay.AddRequestData("vnp_Locale", string.IsNullOrEmpty(model.Locale) ? vnp_Locale : model.Locale);
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnpay.AddRequestData("vnp_IpAddr", vnpay.GetIpAddress(context));
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_Bill_Mobile", "");
            vnpay.AddRequestData("vnp_Bill_Email", model.Email ?? "");


            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            if (!string.IsNullOrEmpty(model.Username))
            {
                var nameParts = model.Username.Trim().Split(' ', 2);
                vnpay.AddRequestData("vnp_Bill_FirstName", nameParts.Length > 0 ? nameParts[0] : "");
                vnpay.AddRequestData("vnp_Bill_LastName", nameParts.Length > 1 ? nameParts[1] : "");
            }

            vnpay.AddRequestData("vnp_Bill_Country", "Vietnam");
            vnpay.AddRequestData("vnp_Bill_Address", "");
            vnpay.AddRequestData("vnp_Bill_City", "");
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
            return response;
        }
    }

}
