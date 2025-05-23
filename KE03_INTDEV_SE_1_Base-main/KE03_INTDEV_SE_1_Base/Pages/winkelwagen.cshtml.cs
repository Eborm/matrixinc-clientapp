using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Newtonsoft.Json;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class winkelwagenModel : PageModel
    {
        private readonly ILogger<winkelwagenModel> _logger;
        private readonly IProductRepository _ProductRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }
        public IList<Product> Products { get; set; }
        public IDictionary<int, int> Cart { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        /// Opbouw dict --> Product_id, Product_count

        public winkelwagenModel(ILogger<winkelwagenModel> logger, IProductRepository ProductRepository, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _ProductRepository = ProductRepository;
            _httpContextAccessor = httpContextAccessor;
            Products = new List<Product>();
            Cart = _httpContextAccessor.HttpContext?.Session.GetObjectFromJson<Dictionary<int, int>>("Cart") ?? new Dictionary<int, int>();
        }

        public void OnGet()
        {
            foreach (var item in Cart)
            {
                var product = _ProductRepository.GetProductById(item.Key);
                if (product != null)
                {
                    Products.Add(product);
                }

            }
            foreach (var Product in Products)
            {
                TotalPrice += Product.Price * Cart[Product.Id];
            }
            TotalPrice += 1;
            _logger.LogInformation($"Totaal prijs = {TotalPrice}");
            _logger.LogInformation($"getting all {Products.Count} Products");
        }

        public IActionResult OnPostOrder()
        {
            var user_id = HttpContext.Session.GetObjectFromJson<int?>("User_id") ?? -1;
            if (user_id == -1)
            {
                return RedirectToPage("login", new { display_message = "login" });
            }
            return RedirectToPage("order", new { });
        }
        public void OnPostRemove(int Id)
        {
            Cart.Remove(Id);
            HttpContext.Session.SetObjectAsJson("Cart", Cart);
        }
    }
}
