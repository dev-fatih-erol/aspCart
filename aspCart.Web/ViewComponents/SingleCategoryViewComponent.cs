using System;
using System.Collections.Generic;
using System.Linq;
using aspCart.Core.Domain.Catalog;
using aspCart.Core.Interface.Services.Catalog;
using aspCart.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace aspCart.Web.ViewComponents
{
    [ViewComponent(Name = "SingleCategory")]
    public class SingleCategoryViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;

        public SingleCategoryViewComponent(
            ICategoryService categoryService,
            IProductService productService,
            IReviewService reviewService,
            IMapper mapper)
        {
            _categoryService = categoryService;
            _productService = productService;
            _reviewService = reviewService;
            _mapper = mapper;
        }

        public IViewComponentResult Invoke(string name)
        {
            var category = _categoryService.GetByName(name);

            ViewBag.Category = category.Name;

            ViewBag.Featured = GetRandomProducts(category.Id);
            ViewBag.Discounted = GetDiscountedProducts(category.Id);
            ViewBag.BestSellers = GetRandomProducts(category.Id);
            ViewBag.OurPicks = GetRandomProducts(category.Id);

            return View();
        }

        private List<ProductModel> GetRandomProducts(Guid categoryId)
        {
            var productEntities = _productService.GetAllProducts()
                .Where(x => x.Categories.Any(p => p.CategoryId == categoryId) && x.Published == true)
                .OrderBy(p => Guid.NewGuid())
                .Take(6);

            var productList = new List<ProductModel>();

            foreach (var product in productEntities)
            {
                var productModel = _mapper.Map<Product, ProductModel>(product);

                productModel.Categories = product.Categories
                    .Select(x => new CategoryModel { Name = x.Category.Name, SeoUrl = x.Category.SeoUrl })
                    .ToList();

                // get main image
                if (product.Images.Count > 0)
                {
                    productModel.MainImage = product.Images
                        .OrderBy(x => x.SortOrder)
                        .ThenBy(x => x.Position)
                        .FirstOrDefault()
                        .Image
                        .FileName;
                }

                // check for discount
                if (product.SpecialPriceEndDate != null && product.SpecialPriceEndDate >= DateTime.Now)
                {
                    productModel.OldPrice = product.Price;
                    productModel.Price = product.SpecialPrice ?? productModel.OldPrice;
                }

                //get product rating
                var reviews = _reviewService.GetReviewsByProductId(productModel.Id);
                if (reviews != null && reviews.Count > 0)
                {
                    productModel.Rating = reviews.Sum(x => x.Rating);
                    productModel.Rating /= reviews.Count;
                }

                productList.Add(productModel);
            }

            /// Start silinecek 
            var proc = productList.Take(3).ToList();
            productList.AddRange(proc);
            /// End silinecek

            return productList;
        }

        private List<ProductModel> GetDiscountedProducts(Guid categoryId)
        {
            var productEntities = _productService.GetAllProducts()
                .Where(x => x.Categories
                .Any(p => p.CategoryId == categoryId) && x.Published == true &&
                                                  x.SpecialPriceEndDate != null &&
                                                  x.SpecialPriceEndDate >= DateTime.Now)
                .OrderBy(p => Guid.NewGuid())
                .Take(6)
                .ToList();

            var productList = new List<ProductModel>();

            foreach (var product in productEntities)
            {
                var productModel = _mapper.Map<Product, ProductModel>(product);

                productModel.Categories = product.Categories
                    .Select(x => new CategoryModel { Name = x.Category.Name, SeoUrl = x.Category.SeoUrl })
                    .ToList();

                // get main image
                if (product.Images.Count > 0)
                {
                    productModel.MainImage = product.Images
                        .OrderBy(x => x.SortOrder)
                        .ThenBy(x => x.Position)
                        .FirstOrDefault()
                        .Image
                        .FileName;
                }

                // check for discount
                if (product.SpecialPriceEndDate != null && product.SpecialPriceEndDate >= DateTime.Now)
                {
                    productModel.OldPrice = product.Price;
                    productModel.Price = product.SpecialPrice ?? productModel.OldPrice;
                }

                //get product rating
                var reviews = _reviewService.GetReviewsByProductId(productModel.Id);
                if (reviews != null && reviews.Count > 0)
                {
                    productModel.Rating = reviews.Sum(x => x.Rating);
                    productModel.Rating /= reviews.Count;
                }

                productList.Add(productModel);
            }

            /// Start silinecek 
            var proc1 = productList.Take(1).ToList();
            var proc2 = productList.Take(1).ToList();
            var proc3 = productList.Take(1).ToList();
            var proc4 = productList.Take(1).ToList();
            var proc5 = productList.Take(1).ToList();
            productList.AddRange(proc1);
            productList.AddRange(proc2);
            productList.AddRange(proc3);
            productList.AddRange(proc4);
            productList.AddRange(proc5);
            /// End silinecek

            return productList;
        }
    }
}