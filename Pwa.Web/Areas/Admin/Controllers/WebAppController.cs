﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pwa.Application.Contracts.Product.Category;
using Pwa.Application.Contracts.Product.WebApplication;
using System.Threading.Tasks;

namespace Pwa.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WebAppController : Controller
    {
        private readonly IWebApplicationApplication _webApplication;
        private readonly ICategoryApplication _categoryApplication;
        public WebAppController(IWebApplicationApplication webApplication, ICategoryApplication categoryApplication)
        {
            _webApplication = webApplication;
            _categoryApplication = categoryApplication;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _webApplication.List();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _categoryApplication.List();

            CreateWebApplicationDto model = new()
            {
                Categories = new SelectList(categories, "Id", "Title")
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateWebApplicationDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _webApplication.Create(dto);
                if (result.Success)
                    return RedirectToAction("Index");
                 
                ModelState.AddModelError("WebSiteAddress", result.Message);
            }
            return View(dto); ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ; ;
        }
    }
}
