using System;
using System.Collections.Generic;
using System.Linq;
using aspCart.Core.Domain.Catalog;
using aspCart.Core.Interface.Services.Catalog;
using aspCart.Core.Interface.Services.Sale;
using aspCart.Infrastructure.EFModels;
using aspCart.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace aspCart.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            IOrderService orderService,
            IProductService productService,
            IReviewService reviewService,
            IMapper mapper)
        {
            _userManager = userManager;
            _orderService = orderService;
            _productService = productService;
            _reviewService = reviewService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductInfo(string seo)
        {
            if (seo != null)
            {
                var productEntity = _productService.GetProductBySeo(seo);
                if (productEntity != null)
                {
                    var productModel = _mapper.Map<Product, ProductModel>(productEntity);
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

                    // get image
                    if (product.Images.Count > 0)
                    {
                        productModel.MainImage = product.Images
                            .OrderBy(x => x.SortOrder)
                            .ThenBy(x => x.Position)
                            .FirstOrDefault()
                            .Image.FileName;
                    }

                    // get manufacturer
                    if (product.Manufacturers.Count > 0)
                    {
                        foreach (var m in product.Manufacturers)
                        {
                            productModel.Manufacturers.Add(new ManufacturerModel { Name = m.Manufacturer.Name, SeoUrl = m.Manufacturer.SeoUrl });
                        }
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
                ViewData["Category"] = category;

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

                    // get all categories
                    foreach (var c in product.Categories)
                    {
                        productModel.Categories.Add(new CategoryModel { Name = c.Category.Name, SeoUrl = c.Category.SeoUrl });
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

                var result = productList;

                // filter the result using category parameter
                if (category.Length > 0)
                {
                    result = result.Where(x => x.Categories.Select(c => c.SeoUrl).Intersect(category).Count() > 0).ToList();
                }

                // filter the result using price parameter
                if (price.Length > 0)
                {
                    var tmpResult = new List<ProductModel>();
                    foreach (var p in price)
                    {
                        var tmpPrice = p.Split(new char[] { '-' });
                        int minPrice = Convert.ToInt32(tmpPrice[0]);
                        int maxPrice = Convert.ToInt32(tmpPrice[1]);

                        var r = result.Where(x => x.Price >= minPrice && x.Price <= maxPrice).ToList();

                        if (r.Count > 0) { tmpResult.AddRange(r); }
                    }
                    result = tmpResult;
                }

                // sort result if the parameter is provided
                if (sortBy != null && sortBy.Length > 0)
                {
                    SortProductModel(sortBy, ref result);
                }

                // get all filters to recheck all filters in view
                var allFilters = category.Concat(price).ToList();
                ViewData["SortKey"] = allFilters;
                ViewData["Manufacturer"] = manufacturer;

                return View(result);
            }

            return RedirectToAction("Index", "Home");
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