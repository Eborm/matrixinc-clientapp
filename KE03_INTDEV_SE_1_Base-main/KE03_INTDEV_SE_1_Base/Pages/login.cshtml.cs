using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KE03_INTDEV_SE_1_Base.Pages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class loginModel : PageModel
    {
        private readonly ILogger<loginModel> _logger;
        private readonly ICustomerRepository _UserRepository;

        public IList<Customer> Users { get; set; }

        public string DisplayMessage { get; set; }

        public loginModel(ILogger<loginModel> logger, ICustomerRepository UserRepository)
        {
            _logger = logger;
            _UserRepository = UserRepository;
            Users = new List<Customer>();
        }
        public void OnGet(int? setId)
        {
            Users = _UserRepository.GetAllCustomers().ToList();
            _logger.LogInformation($"getting all {Users.Count} Users");
            DisplayMessage = Request.Query["display_message"];
            _logger.LogInformation("Display message: {DisplayMessage}", DisplayMessage);

            if (setId.HasValue)
            {
                HttpContext.Session.SetObjectAsJson("User_id", setId.Value);
                _logger.LogInformation("User ID {UserId} stored in session via GET.", setId.Value);
            }
        }

        public void OnPostSetAccount(int id)
        {
            HttpContext.Session.SetObjectAsJson("User_id", id);
            _logger.LogInformation("User ID {UserId} stored in session.", id);
        }

        public int GetUserId()
        {
            return HttpContext.Session.GetObjectFromJson<int?>("User_id") ?? -1;
        }
    }
}
