﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectPRNLamnthe180410.Hubs;
using ProjectPRNLamnthe180410.Models;
using ProjectPRNLamnthe180410.Services.Interface;

namespace ProjectPRNLamnthe180410.Controllers
{
    public class DetailsController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly ILightNovelService _lightNovelService;
        private readonly IHubContext<LightNovelHub> _hubContext;
        private readonly IUserService _userService;
        private readonly ICommentService _commentService;

        public DetailsController(
            IGenreService genreService,
            ILightNovelService lightNovelService,
            IHubContext<LightNovelHub> hubContext,
            IUserService userService,
            ICommentService commentService)
        {
            _genreService = genreService;
            _lightNovelService = lightNovelService;
            _hubContext = hubContext;
            _userService = userService;
            _commentService = commentService;
        }
        public async Task<IActionResult> Index(int id)
        {
            var user = new User();
            var lightNovel = await _lightNovelService.GetLightNovelByIdAsync(id);
            if (lightNovel == null)
            {
                return NotFound();
            }
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId != null)
            {
                user = await _userService.GetUserByIdAsync((int)userId);
                bool hasBought = await _userService.HasUserBoughtLightNovelAsync((int)userId, id);

                ViewData["User"] = user;
                ViewData["HasBought"] = hasBought;

            }
            else
            {
                ViewData["User"] = null;
            }
            var genre = await _genreService.GetGenreByIdAsync(lightNovel.GenreId);
            var comments = await _commentService.GetCommentsByLightNovelIdAsync(id);

            ViewData["Comments"] = comments;

            ViewData["LightNovel"] = lightNovel;
            ViewData["Genre"] = genre;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(int lightNovelId, string content)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            var user = await _userService.GetUserByIdAsync((int)userId);

            var newComment = await _commentService.AddCommentAsync(lightNovelId, (int)userId, content);

            // Notify all clients via SignalR

            await _hubContext.Clients.All.SendAsync("ReceiveUpdate",
                newComment.Id,
                user != null ? user.Username : "Anonymous",
                $"{newComment.Content}",
                DateTime.UtcNow.ToString("o"));

            return RedirectToAction("Index", new { id = lightNovelId });
        }
        [HttpPost]
        public async Task<IActionResult> Buy(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null || userId == -1)
            {
                return RedirectToAction("Index", "Login");
            }

            var lightNovel = await _lightNovelService.GetLightNovelByIdAsync(id);
            if (lightNovel == null)
            {
                TempData["Error"] = "Light novel not found.";
                return RedirectToAction("Index", new { id });
            }

            var canBuy = await _lightNovelService.CheckCanBuyAsync((int)userId, id);
            if (!canBuy)
            {
                TempData["Error"] = "Insufficient coins to purchase this light novel.";
                return RedirectToAction("Index", new { id });
            }

            try
            {
                // Deduct coins
                await _userService.UpdateCoinsAsync((int)userId, (int)-lightNovel.Cost);

                // Record purchase in Bought table
                await _lightNovelService.RecordPurchaseAsync((int)userId, id);

                // Notify via SignalR with a purchase message in comment-like format
                var user = await _userService.GetUserByIdAsync((int)userId);
                await _hubContext.Clients.All.SendAsync("ReceiveUpdate",
                    0, // Dummy ID to indicate purchase (comments use real IDs)
                    user.Username,
                    $"purchased {lightNovel.Title}", // Content indicates a purchase
                    DateTime.UtcNow.ToString("o"));

                TempData["Success"] = $"Successfully purchased {lightNovel.Title}!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred during purchase.";
            }

            return RedirectToAction("Index", new { id });
        }
    }
}

