﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pwa.Application.Contracts.Product.Comment;
using Pwa.Infrastructure.EfCore;
using Pwa.Query.Contracts.WebApp;
using System.Collections.Generic;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using WebFramework;
using WebFramework.Domain;
using WebFramework.Utilities.Sms;

namespace Pwa.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebAppQuery _webApp;
        private readonly ICommentApplication _comment;
        private readonly ISmsService _sms;
        public HomeController(ISmsService sms, IWebAppQuery webApp, ICommentApplication comment)
        {
            _sms = sms;
            _webApp = webApp;
            _comment = comment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InviteFriend(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return View();
            }
            await _sms.Send(phoneNumber, "شما به داریا وب اپ دعوت شده اید!");
            return RedirectToAction("Index");
        }

        [Route("single/{id}")]
        public async Task<IActionResult> Single(int id)
        {
            var webApp = await _webApp.GetSingle(id);
            if (webApp.Success is false)
                return NotFound();
            webApp.Data.Comment = new CreateCommentDto
            {
                WebApplicationId = webApp.Data.Id
            };
            return View(webApp.Data);
        }

        [HttpPost("single/{id}")]
        public async Task<IActionResult> Single(CreateCommentDto comment, int id, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var result = await _comment.Add(comment, cancellationToken);
                if (result.Success)
                    return RedirectToAction("Single", new { Id = id });
            }
            return View(comment);
        }

        [Route("list/{page}.{search?}")]
        public async Task<IActionResult> List(int page = 1, [FromQuery] string search = "", [FromQuery] string sortOrder = "")
        {
            ResponseDto<WebAppQueryModel> response = new()
            {
                Page = page,
                Search = search,
                SortOrder = sortOrder
            };

            var webApps = await _webApp.List(response);

            ViewBag.Pager = webApps.Pager;

            ViewBag.SearchQuery = search;

            return View(webApps.Items);
        }

        [Authorize]
        public async Task<IActionResult> Install(int id, CancellationToken cancellationToken)
        {
            var address = await _webApp.Install(id, cancellationToken);
            if (address.Success is false) return NotFound();
            return Redirect(address.Data);
        }
    }
}
