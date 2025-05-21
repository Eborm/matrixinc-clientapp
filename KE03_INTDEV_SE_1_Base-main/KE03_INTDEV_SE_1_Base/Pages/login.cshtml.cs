using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class loginModel : PageModel
    {
        private readonly ILogger<loginModel> _logger;
        private readonly ICustomerRepository _UserRepository;

        public IList<Customer> Users { get; set; }

        public loginModel(ILogger<loginModel> logger, ICustomerRepository UserRepository)
        {
            _logger = logger;
            _UserRepository = UserRepository;
            Users = new List<Customer>();
        }
        public void OnGet()
        {
            Users = _UserRepository.GetAllCustomers().ToList();
            _logger.LogInformation($"getting all {Users.Count} Users");
        }
    }
}
