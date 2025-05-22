using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KE03_INTDEV_SE_1_Base.Pages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class topbarModel : PageModel
    {
        private readonly ILogger<topbarModel> _logger;
        private readonly ICustomerRepository _UserRepository;

        public IList<Customer> Users { get; set; }

        public topbarModel(ILogger<topbarModel> logger, ICustomerRepository UserRepository)
        {
            _logger = logger;
            _UserRepository = UserRepository;
            Users = new List<Customer>();
        }
        public void OnGet(int? setId)
        {
            Users = _UserRepository.GetAllCustomers().ToList();
            _logger.LogInformation($"getting all {Users.Count} Users");
        }

        public int GetUserId()
        {
            return HttpContext.Session.GetObjectFromJson<int?>("User_id") ?? -1;
        }
    }
}