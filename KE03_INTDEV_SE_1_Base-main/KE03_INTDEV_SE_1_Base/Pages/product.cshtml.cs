using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class productModel : PageModel
    {
        private readonly ILogger<productModel> _logger;
        private readonly IProductRepository _ProductRepository;

        public IList<Product> Products { get; set; }

        public productModel(ILogger<productModel> logger, IProductRepository ProductRepository)
        {
            _logger = logger;
            _ProductRepository = ProductRepository;
            Products = new List<Product>();
        }

        public void OnGet()
        {
            Products = _ProductRepository.GetAllProducts().ToList();
            _logger.LogInformation($"getting all {Products.Count} customers");
        }
    }
}
