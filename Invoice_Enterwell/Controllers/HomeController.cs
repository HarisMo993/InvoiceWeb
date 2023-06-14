using Invoice.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Invoice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Prikazi sve njegove napravljene fakture
                    var invoices = _dbContext.BillInvoices.Where(i => i.CreatorUserId == user.Id).ToList();
                    return View(invoices);
                }
            }

            // Korisnik nije prijavljen, prikaži poruku ili preusmjeri na prijavu
            ViewBag.Message = "Morate biti prijavljeni kako biste vidjeli i kreirali fakture.";
            return View();
        }
    }
}
