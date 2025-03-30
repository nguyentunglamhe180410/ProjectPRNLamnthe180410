using Microsoft.AspNetCore.Mvc;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Models.ViewModel;
using ProjectPRNLamnthe180410.Services.Interface;

namespace ProjectPRNLamnthe180410.Controllers
{
    public class BuyController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly IUserService _userService; // Replace with your actual user service interface

        public BuyController(IVnPayService vnPayService, IUserService userService)
        {
            _vnPayService = vnPayService ?? throw new ArgumentNullException(nameof(vnPayService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // GET: Buy/Index
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId != null && userId != -1)
            {
                var user = await _userService.GetUserByIdAsync((int)userId);
                ViewData["User"] = user;

                var model = new PaymentResponseModel
                {
                    Amount = 0 // Default value, user will input this
                };
                return View(model);
            }

            // If no user is logged in, redirect to login
            return RedirectToAction("Index", "Login");
        }

        // POST: Buy/ProcessPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(PaymentResponseModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null || userId == -1)
            {
                return RedirectToAction("Index", "Login");
            }

            // Fetch user details from session
            var currentUser = await _userService.GetUserByIdAsync((int)userId);
            model.Username = currentUser.Username;
            model.Email = currentUser.Email;
            model.ContentTitle = $"{model.Username} purchase {model.Amount} coins";

            // Generate VnPay payment URL
            string paymentUrl = _vnPayService.CreatePaymentUrl(model, HttpContext);
            History history = new History();
            history.UserId = (int)userId;
            history.PurchasedDay = DateTime.UtcNow;
            history.Amount = model.Amount;
            history.Status = false;
            history.User = currentUser;
            await _userService.AddHistoryAsync(history);
            HttpContext.Session.SetInt32("HistoryInfo", history.Id);
            // Redirect to VnPay payment page
            return Redirect(paymentUrl);
        }

        // GET: Buy/Return (Callback from VnPay after payment)
        public async Task<IActionResult> ReturnAsync()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            int? userId = HttpContext.Session.GetInt32("UserID");
            History history = new History();
            try
            {
                int historyID = (int)HttpContext.Session.GetInt32("HistoryInfo");
                history = await _userService.GetHistoryByIdAsync(historyID);
            }
            catch (Exception ex) {
                TempData["Message"] = "Dữ liệu đơn hàng không hợp lệ. Vui lòng thử lại.";
                return RedirectToAction("Buy");
            }
            if (userId != null && userId != -1)
            {
                var user = await _userService.GetUserByIdAsync((int)userId);
                ViewData["User"] = user;

                if (response.ResponseCode == "00")
                {
                    // Payment successful, process the coin purchase
                    TempData["Message"] = $"Successfully purchased {response.Amount} coins for {response.Username}!";
                    await _userService.UpdateCoinsAsync((int)userId, response.Amount);
                    history.Status = true;
                    await _userService.UpdateHistoryAsync(history);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    history.Status = false;
                    await _userService.UpdateHistoryAsync(history);
                    TempData["Message"] = $"Thanh toán thất bại. Mã lỗi: {response.ResponseCode}. Vui lòng thử lại.";
                    ViewData["Response"] = response;
                }
                // Payment failed
                TempData["Error"] = "Payment failed or was canceled.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "User not log in";
                return RedirectToAction("Index", "Login");
            }

        }
    }
}
