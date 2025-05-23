using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Models;
using KE03_INTDEV_SE_1_Base.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class orderModel : PageModel
    {
        private readonly ILogger<orderModel> _logger;
        private readonly IProductRepository _ProductRepository;
        private readonly ICustomerRepository _CustomerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }
        public IList<Product> Products { get; set; }
        public IDictionary<int, int> Cart { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        /// Opbouw dict --> Product_id, Product_count
        /// 
        public int user_id { get; set; }

        private readonly MatrixIncDbContext _context;
        
        public Customer user;

        public orderModel(MatrixIncDbContext context, ILogger<orderModel> logger, IProductRepository ProductRepository, IHttpContextAccessor httpContextAccessor, ICustomerRepository CustomerRepository)
        {
            _logger = logger;
            _ProductRepository = ProductRepository;
            _httpContextAccessor = httpContextAccessor;
            _CustomerRepository = CustomerRepository;
            Products = new List<Product>();
            Cart = _httpContextAccessor.HttpContext?.Session.GetObjectFromJson<Dictionary<int, int>>("Cart") ?? new Dictionary<int, int>();
            user_id = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<int?>("User_id") ?? -1;
            user = CustomerRepository.GetCustomerById(user_id);
            _context = context;
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

        public IActionResult OnPostOrder(string naam, string address, string id, string? email)
        {
            Order order = new Order();
            if (Products.Count() < 1)
            {
                return RedirectToPage("index", new { });
            }
            foreach (var entry in Cart)
            {
                var product = _ProductRepository.GetProductById(entry.Key);
                if (product != null)
                {
                    order.OrderProducts.Add(new OrderProduct
                    {
                        Product = product,
                        ProductId = product.Id,
                        Quantity = entry.Value
                    });
                }
            }

            order.CustomerId = int.Parse(id);
            order.OrderDate = DateTime.Now;
            if (user_id == 0)
            {
                order.Address = address;
                order.Email = email;
            }
            else if (user.Address != address)
            {
                order.Address = address;
            }

            var productInfo = string.Join(", ", order.Products.Select(p => $"{p.Name} (ID: {p.Id}, Price: €{p.Price:0.00})"));

            _logger.LogInformation(
                $"Order aangemaakt: " +
                $"CustomerId={order.CustomerId}, " +
                $"Address='{order.Address}', " +
                $"Email='{order.Email}', " +
                $"OrderDate={order.OrderDate}, " +
                $"Products=[{productInfo}]");

            HttpContext.Session.SetObjectAsJson("Cart", new Dictionary<int, int>());
            _context.Orders.Add(order);
            _context.SaveChanges();
            _context.Database.EnsureCreated();

            return RedirectToPage("order", new { });
        }
    }
}
