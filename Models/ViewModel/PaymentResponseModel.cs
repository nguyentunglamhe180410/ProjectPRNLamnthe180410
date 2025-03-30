namespace ProjectPRNLamnthe180410.Models.ViewModel
{
    public class PaymentResponseModel
    {
        public int Amount { get; set; } // Changed from decimal to int
        public string ContentTitle { get; set; } // Will be formatted as "Username purchase amount coins"
        public string Username { get; set; } // Changed from ReaderName
        public string Email { get; set; }
        public string Locale { get; set; }
        public string? ResponseCode { get; set; }
        public int? HistoryID { get; set; }
    }
}
