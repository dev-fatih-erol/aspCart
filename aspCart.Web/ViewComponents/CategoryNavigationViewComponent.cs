using System;
using System.Linq;
using aspCart.Core.Domain.Catalog;
using aspCart.Core.Interface.Services.Catalog;
using aspCart.Web.Models;
using Microsoft.AspNetCore.Mvc;
using aspCart.Web.Extensions;

namespace aspCart.Web.ViewComponents
{
    [ViewComponent(Name = "CategoryNavigation")]
    public class CategoryNavigationViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public CategoryNavigationViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IViewComponentResult Invoke(Guid categoryId, string productName)
        {
            var categories = _categoryService.GetAllCategories();

            var current = categories.Where(c => c.Id == categoryId).Single();

            var parents = categories
                .Parents(current, x => x.Id, x => x.ParentCategoryId)
                .Reverse<Category>()
                .ToList();

            parents.Add(current);

            ViewBag.ProductName = productName;

            return View(parents.Select(x => new CategoryModel
            {
                Name = x.Name,
                SeoUrl = x.SeoUrl
            }).ToList());
        }
    }
}