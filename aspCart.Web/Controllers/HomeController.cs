﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspCart.Core.Domain.Catalog;
using aspCart.Core.Interface.Services.Catalog;
using aspCart.Core.Interface.Services.Sale;
using aspCart.Infrastructure.EFModels;
using aspCart.Web.Extensions;
using aspCart.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace aspCart.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            IOrderService orderService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IReviewService reviewService,
            IMapper mapper)
        {
            _userManager = userManager;
            _orderService = orderService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _productService = productService;
            _reviewService = reviewService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var productEntities = _productService.GetAllProducts()
                .Where(x => x.SpecialPriceEndDate != null &&
                            x.SpecialPriceEndDate >= DateTime.Now)
                .OrderBy(p => Guid.NewGuid())
                .Take(9)
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
            var proc1 = productList.Take(3).ToList();
            var proc2 = productList.Take(3).ToList();
            productList.AddRange(proc1);
            productList.AddRange(proc2);
            /// End silinecek
            
            return View(productList);
        }

        public IActionResult ProductInfo(string seo)
        {
            if (seo != null)
            {
                var productEntity = _productService.GetProductBySeo(seo);
                if (productEntity != null)
                {
                    var productModel = _mapper.Map<Product, ProductModel>(productEntity);

                    productModel.Categories = productEntity.Categories
                        .Select(x => new CategoryModel { Name = x.Category.Name, SeoUrl = x.Category.SeoUrl })
                        .ToList();

                    productModel.Description = System.Net.WebUtility.HtmlDecode(productModel.Description);

                    // check for discount
                    if (productEntity.SpecialPriceEndDate != null && productEntity.SpecialPriceEndDate >= DateTime.Now)
                    {
                        productModel.OldPrice = productEntity.Price;
                        productModel.Price = productEntity.SpecialPrice ?? productModel.OldPrice;
                    }

                    // get all images
                    if (productEntity.Images.Count > 0)
                    {
                        productModel.MainImage = productEntity.Images
                            .OrderBy(x => x.SortOrder)
                            .ThenBy(x => x.Position)
                            .FirstOrDefault()
                            .Image.FileName;

                        productModel.ProductImages = productEntity.Images.Select(x => x.Image.FileName).ToList();
                    }

                    // get all manufacturers
                    var manufacturers = productEntity.Manufacturers;
                    foreach (var manufacturer in manufacturers)
                    {
                        productModel.Manufacturers.Add(new ManufacturerModel
                        {
                            Name = manufacturer.Manufacturer.Name,
                            SeoUrl = manufacturer.Manufacturer.SeoUrl
                        });
                    }

                    // get all specifications
                    var specifications = productEntity.Specifications.OrderBy(x => x.SortOrder).ThenBy(x => x.Position);
                    foreach (var specification in specifications)
                    {
                        productModel.Specifications.Add(new SpecificationModel
                        {
                            Key = specification.Specification.Name,
                            Value = System.Net.WebUtility.HtmlDecode(specification.Value),
                            SortOrder = specification.SortOrder
                        });
                    }

                    //get product rating
                    var reviews = _reviewService.GetReviewsByProductId(productModel.Id);
                    if (reviews != null && reviews.Count > 0)
                    {
                        productModel.Rating = reviews.Sum(x => x.Rating);
                        productModel.Rating /= reviews.Count;
                        productModel.ReviewCount = reviews.Count;
                    }

                    ViewData["ProductId"] = productModel.Id;
                    if (User.Identity.IsAuthenticated)
                        ViewData["ProductReviewer"] = _reviewService.GetReviewByProductIdUserId(productModel.Id, GetCurrentUserId()) != null ? true : false;

                    ViewBag.CategoryId = productEntity.Categories.First().CategoryId;
                    return View(productModel);
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult ProductCategory(string category, [FromQuery(Name = "siralama")] string sortBy, [FromQuery(Name = "marka")] string[] manufacturer, [FromQuery(Name = "fiyat")] string[] price)
        {
            if (category != null)
            {
                var productEntities = _productService.SearchProduct(
                    categoryFilter: new string[] { category },
                    manufacturerFilter: manufacturer,
                    priceFilter: price);

                var productList = new List<ProductModel>();

                foreach (var product in productEntities)
                {
                    var productModel = _mapper.Map<Product, ProductModel>(product);

                    productModel.Categories = product.Categories
                        .Select(x => new CategoryModel { Name = x.Category.Name, SeoUrl = x.Category.SeoUrl })
                        .ToList();

                    // get image
                    if (product.Images.Count > 0)
                    {
                        productModel.MainImage = product.Images
                            .OrderBy(x => x.SortOrder)
                            .ThenBy(x => x.Position)
                            .FirstOrDefault()
                            .Image.FileName;
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
                        productModel.ReviewCount = reviews.Count;
                    }

                    productList.Add(productModel);
                }

                // sort result if the parameter is provided
                if (sortBy != null && sortBy.Length > 0)
                {
                    SortProductModel(sortBy, ref productList);
                }

                // get all filters to recheck all filters in view
                var allFilters = manufacturer.Concat(price).ToList();
                ViewData["SortKey"] = allFilters;

                var currentCategory = _categoryService.GetCategoryBySeo(category);
                ViewData["Category"] = currentCategory;

                var categories = new List<CategoryModel>();
                foreach (var categoryItem in _categoryService.GetAllCategoriesWithoutParent())
                {
                    categories.Add(new CategoryModel
                    {
                        Name = categoryItem.Name,
                        SeoUrl = categoryItem.SeoUrl,
                        ProductCount = _productService.Table()
                                                      .Where(x => x.Categories
                                                      .Any(c => c.CategoryId == categoryItem.Id))
                                                      .Count()
                    });
                }
                ViewData["Categories"] = categories;

                var allCategories = _categoryService.GetAllCategories();

                var current = allCategories.Where(c => c.Id == currentCategory.Id).Single();

                var parents = allCategories
                    .Parents(current, x => x.Id, x => x.ParentCategoryId)
                    .Reverse<Category>()
                    .ToList();

                parents.Add(current);

                ViewData["CategoryNavigation"] = parents.Select(x => new CategoryModel
                {
                    Name = x.Name,
                    SeoUrl = x.SeoUrl
                }).ToList();

                var subCategories = new List<CategoryModel>();
                foreach (var categoryItem in _categoryService.Table().Where(c => c.ParentCategoryId == currentCategory.Id).ToList())
                {
                    subCategories.Add(new CategoryModel
                    {
                        Name = categoryItem.Name,
                        SeoUrl = categoryItem.SeoUrl,
                        ProductCount = _productService.Table()
                                                      .Where(x => x.Categories
                                                      .Any(c => c.CategoryId == categoryItem.Id))
                                                      .Count()
                    });
                }
                ViewData["SubCategories"] = subCategories;

                var currentCategoryData = new CategoryModel
                {
                    Name = currentCategory.Name,
                    SeoUrl = currentCategory.SeoUrl,
                    ProductCount = _productService.Table()
                                                      .Where(x => x.Categories
                                                      .Any(c => c.CategoryId == currentCategory.Id))
                                                      .Count()
                };
                ViewData["CurrentCategoryData"] = currentCategoryData;

                return View(productList);
            }

            return RedirectToAction("Index");
        }

        public IActionResult ProductManufacturer(string manufacturer, [FromQuery(Name = "siralama")] string sortBy, [FromQuery(Name = "kategori")] string[] category, [FromQuery(Name = "fiyat")] string[] price)
        {
            if (manufacturer != null)
            {
                var productEntities = _productService.SearchProduct(
                    categoryFilter: category,
                    manufacturerFilter: new string[] { manufacturer },
                    priceFilter: price);

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
                        productModel.ReviewCount = reviews.Count;
                    }

                    productList.Add(productModel);
                }

                // sort result if the parameter is provided
                if (sortBy != null && sortBy.Length > 0)
                {
                    SortProductModel(sortBy, ref productList);
                }

                // get all filters to recheck all filters in view
                var allFilters = category.Concat(price).ToList();
                ViewData["SortKey"] = allFilters;

                var currentManufacturer = _manufacturerService.GetManufacturerBySeo(manufacturer);
                ViewData["Manufacturer"] = currentManufacturer;

                var manufacturers = new List<ManufacturerModel>();
                foreach (var manufacturerItem in _manufacturerService.GetAllManufacturers())
                {
                    manufacturers.Add(new ManufacturerModel
                    {
                        Name = manufacturerItem.Name,
                        SeoUrl = manufacturerItem.SeoUrl,
                        ProductCount = _productService.Table()
                                                      .Where(x => x.Manufacturers
                                                      .Any(c => c.ManufacturerId == manufacturerItem.Id))
                                                      .Count()
                    });
                }
                ViewData["Manufacturers"] = manufacturers;

                var currentManufacturerData = new ManufacturerModel
                {
                    Name = currentManufacturer.Name,
                    SeoUrl = currentManufacturer.SeoUrl,
                    ProductCount = _productService.Table()
                                                      .Where(x => x.Manufacturers
                                                      .Any(c => c.ManufacturerId == currentManufacturer.Id))
                                                      .Count()
                };
                ViewData["CurrentManufacturerData"] = currentManufacturerData;

                return View(productList);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ProductReview(string id)
        {
            var model = new List<ReviewModel>();
            if (id != null && id.Length > 0)
            {
                Guid result = Guid.Empty;

                if (Guid.TryParse(id, out result))
                {
                    var reviewEntities = _reviewService.GetReviewsByProductId(result);
                    if (reviewEntities != null && reviewEntities.Count > 0)
                    {
                        foreach (var review in reviewEntities)
                        {
                            var r = _mapper.Map<Review, ReviewModel>(review);

                            var user = await _userManager.FindByIdAsync(review.UserId.ToString());
                            r.Username = user.Name + " " + user.Surname[0] + ".";

                            r.IsVerifiedOwner = _orderService.GetAllOrdersByUserId(review.UserId)
                                .Where(x => x.Items.Any(a => a.ProductId == result.ToString()) && x.Status == Core.Domain.Sale.OrderStatus.Complete)
                                .Count() > 0;

                            model.Add(r);
                        }
                    }
                }
            }

            return Json(model.OrderByDescending(x => x.CreatedOn));
        }

        [Authorize]
        public IActionResult CreateReview(string id)
        {
            if (id != null && id.Length > 0)
            {
                Guid result = Guid.Empty;
                var model = new CreateReviewModel();

                if (Guid.TryParse(id, out result))
                {
                    var productEntity = _productService.GetProductById(result);
                    model.ProductId = productEntity.Id;
                    model.ProductName = productEntity.Name;
                    model.ProductSeo = productEntity.SeoUrl;
                    model.Rating = 1;

                    var userReview = _reviewService.GetReviewByProductIdUserId(result, GetCurrentUserId());
                    if (userReview != null)
                    {
                        model.Title = userReview.Title;
                        model.Message = userReview.Message;
                        model.Rating = userReview.Rating;
                        ViewData["ReviewEdited"] = true;
                    }
                }

                return View(model);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateReview(CreateReviewModel model)
        {
            if (ModelState.IsValid)
            {
                var reviewEntity = new Review
                {
                    Id = Guid.NewGuid(),
                    UserId = GetCurrentUserId(),
                    ProductId = model.ProductId,
                    Title = model.Title,
                    Message = model.Message,
                    Rating = model.Rating,
                    CreatedOn = DateTime.Now
                };

                _reviewService.InsertReview(reviewEntity);

                return RedirectToAction("ProductInfo", "Home", new { seo = model.ProductSeo }, "Reviews");
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditReview(CreateReviewModel model)
        {
            if (ModelState.IsValid)
            {
                var reviewEntity = _reviewService.GetReviewByProductIdUserId(model.ProductId, GetCurrentUserId());
                if (reviewEntity != null)
                {
                    reviewEntity.Title = model.Title;
                    reviewEntity.Message = model.Message;
                    reviewEntity.Rating = model.Rating;
                    reviewEntity.DateModified = DateTime.Now;

                    _reviewService.UpdateReview(reviewEntity);
                }

                return RedirectToAction("ProductInfo", "Home", new { seo = model.ProductSeo }, "Reviews");
            }

            return View(model);
        }

        private Guid GetCurrentUserId()
        {
            if (Guid.TryParse(_userManager.GetUserId(HttpContext.User) ?? "", out Guid id))
            {
                return id;
            }

            return id;
        }

        private void SortProductModel(string sortBy, ref List<ProductModel> model)
        {
            switch (sortBy)
            {
                case "endusukfiyat":
                    model = model.OrderBy(x => x.Price)
                        .ThenBy(x => x.Name)
                        .ToList();
                    break;

                case "enyuksekfiyat":
                    model = model.OrderByDescending(x => x.Price)
                        .ThenBy(x => x.Name)
                        .ToList();
                    break;

                case "encoksatan":
                    break;

                case "encokdegerlendirme":
                    model = model.OrderByDescending(x => x.ReviewCount)
                        .ThenBy(x => x.Name)
                        .ToList();
                    break;

                case "enyeniler":
                    model = model.OrderByDescending(x => x.DateAdded)
                        .ThenBy(x => x.Name)
                        .ToList();
                    break;

                default:
                    break;
            }

            ViewData["SortBy"] = sortBy;
        }
    }
}