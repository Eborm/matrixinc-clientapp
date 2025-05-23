using System.Runtime.CompilerServices;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class orderHistoryModel : PageModel
    {
        private readonly ILogger<orderHistoryModel> _logger;
        private readonly IOrderRepository _OrderRepoistory;
        private readonly IProductRepository _ProductRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }
        public IList<Order> Orders { get; set; }

        public int user_id { get; set; } = -1;
        public Dictionary<int, int> ProductCount { get; set; } = new Dictionary<int, int>
        {
            { 0, 0}
        };
        public Dictionary<int, decimal> product_count_extra = new Dictionary<int, decimal> { { 0, 0.00m } };



        public orderHistoryModel(ILogger<orderHistoryModel> logger, IOrderRepository orderRepository, IHttpContextAccessor httpContextAccessor, IProductRepository productRepository)
        {
            _logger = logger;
            _OrderRepoistory = orderRepository;
            _ProductRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public void OnGet()
        {
            user_id = _httpContextAccessor.HttpContext.Session.GetObjectFromJson<int?>("User_id") ?? -1;
            decimal TotalPrice = 0;
            Orders = _OrderRepoistory.GetAllOrders().ToList();
            foreach (var Order in Orders)
            {
                TotalPrice = 1; 
                _logger.LogInformation($"Order ID {Order.Id} has {Order.OrderProducts.Count} OrderProducts");
                int productCount = 0;
                foreach (var OrderProduct in Order.OrderProducts)
                {
                    productCount += 1*OrderProduct.Quantity;
                    _logger.LogInformation($"product count: {OrderProduct.Quantity.ToString()}");
                    TotalPrice += OrderProduct.Product.Price*OrderProduct.Quantity;
                }
                
                _logger.LogInformation($"product count: {productCount.ToString()}");
                product_count_extra[productCount] = TotalPrice;
                ProductCount[Order.Id] = productCount;
            }
            TotalPrice += 1;
            _logger.LogInformation($"Totaal prijs = {TotalPrice}");
            _logger.LogInformation($"getting all {Orders.Count} Products");
        }
    }
}
