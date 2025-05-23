using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Newtonsoft.Json;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class productModel : PageModel
    {
        private readonly ILogger<productModel> _logger;
        private readonly IProductRepository _ProductRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }
        public IList<Product> Products { get; set; }
        public IDictionary<int, int> Cart{ get; set; }
        /// Opbouw dict --> Product_id, Product_count

        public productModel(ILogger<productModel> logger, IProductRepository ProductRepository, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _ProductRepository = ProductRepository;
            _httpContextAccessor = httpContextAccessor;
            Products = new List<Product>();
            Cart = _httpContextAccessor.HttpContext?.Session.GetObjectFromJson<Dictionary<int, int>>("Cart") ?? new Dictionary<int, int>();
        }

        public void OnGet()
        {
            Products = _ProductRepository.GetAllProducts().ToList();
            _logger.LogInformation($"getting all {Products.Count} Products");
        }

        public IActionResult OnPostSetCart(int Product_id, int Product_count)
        {
            if (Cart.ContainsKey(Product_id))
            {
                Cart[Product_id] += Product_count;
            }
            else
            {
                Cart.Add(Product_id, Product_count);
            }

            HttpContext.Session.SetObjectAsJson("Cart", Cart);
            _logger.LogInformation("Set Cart to: {cart}", JsonConvert.SerializeObject(Cart));
            return RedirectToPage("index", new { });
        }
    }
}
