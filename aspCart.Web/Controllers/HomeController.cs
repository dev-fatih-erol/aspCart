using System;
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

        private Guid GetCurrentUserId()
        {
            if (Guid.TryParse(_userManager.GetUserId(HttpContext.User) ?? "", out Guid id))
            {
                return id;
            }

            return id;
        }
    }
}