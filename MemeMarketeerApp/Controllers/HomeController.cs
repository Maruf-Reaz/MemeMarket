using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MemeMarketeerApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MemeMarketeerApp.Models.Common.Authentication;
using MemeMarketeerApp.Data;
using MemeMarketeerApp.Models.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MemeMarketeerApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        //[Authorize(Roles = "GateAdmin, HarbourAndMarine, Mechanical, Admin, TMOffice")]
        public async Task<IActionResult> Index()
        {
            DateTime fromDate = DateTime.Now.AddDays(-30);
            DateTime toDate = DateTime.Now;


            //var user = (await _userManager.FindByNameAsync(HttpContext.User.Identity.Name)); //same thing
            //if (await _userManager.IsInRoleAsync(user, "Admin"))
            //{
            //    ViewData["Merchants"] = await _context.Merchants.Include(m => m.ApplicationUser).Where(m => m.ApplicationUser.Status == 1).ToListAsync();
            //     ViewData["DeliveryMans"] = await _context.DeliveryMans.Where(m => m.Status == 1).ToListAsync();
            //    ViewData["IsAdmin"] = true;


            //}
            //else if (await _userManager.IsInRoleAsync(user, "Merchant"))
            //{
            //    ViewData["Tarrifs"] = await _context.Tarrifs.Include(m => m.PackageCatagory).Include(m => m.LocationFrom).Include(m => m.LocationTo).Where(m => m.Status == 1).ToListAsync();
            //    ViewData["IsAdmin"] = false;
            //}
            ViewData["Affiliates"] = await _context.Affiliates.OrderByDescending(m => m.Id).Take(10)
                    .Include(m => m.AffiliateLanguage)
                    .Include(m => m.Catagory).ToListAsync();
            ViewData["AffiliateCount"] = await _context.Affiliates.CountAsync();
            ViewData["AffiliateCountThisMonth"] = await _context.Affiliates.Where(m =>  m.AddintionDate<= toDate && m.AddintionDate>=fromDate).CountAsync();


             ViewData["Clients"] = await _context.Clients.OrderByDescending(m => m.Id).Take(10).ToListAsync();
            ViewData["ClientCount"] = await _context.Clients.CountAsync();
            ViewData["ClientCountThisMonth"] = await _context.Clients.Where(m => m.AddintionDate<= toDate && m.AddintionDate>=fromDate).CountAsync();

           
            ViewData["CampaignCount"] = await _context.Campaigns.CountAsync();
            ViewData["CampaignCountThisMonth"] = await _context.Campaigns.Where(m => m.CreationDate <= toDate && m.CreationDate >= fromDate).CountAsync();
            var allcampaigns = await _context.Campaigns.Include(m=>m.Client).OrderByDescending(m => m.Id).Take(10).ToListAsync();
            List<CampaignHomeViewModel> campaigns = new List<CampaignHomeViewModel>();

            foreach (var soloCampaign in allcampaigns)
            {
                CampaignHomeViewModel campaign = new CampaignHomeViewModel();
                campaign.Campaign = soloCampaign;
                if (soloCampaign.Status == 1)
                {
                    campaign.TotalExpence = Campaign.GetExpenseForCampaign(soloCampaign.Id, _context);
                    campaign.TotalImpressionGenerated = Campaign.GetImpressionsGenerated(soloCampaign.Id, _context);

                }

                campaigns.Add(campaign);
            }

            ViewData["Campaigns"] = campaigns;




            ViewData["ImpressionCount"] = await _context.Posts.Where(m => m.Campaign.Status == 1).SumAsync(m => m.ImpressionGenerated);
            ViewData["ImpressionCountThisMonth"] = await _context.Posts.Where(m => m.Campaign.CreationDate <= toDate && m.Campaign.CreationDate >= fromDate && m.Campaign.Status == 1).SumAsync(m => m.ImpressionGenerated);
            

             ViewData["LinkClickCount"] = await _context.Campaigns.Where(m => m.Status == 1).SumAsync(m => m.TotalLinkClicked);
            ViewData["LinkClickCountThisMonth"] = await _context.Campaigns.Where(m => m.CreationDate <= toDate && m.CreationDate >= fromDate && m.Status == 1).SumAsync(m => m.TotalLinkClicked);

            
             ViewData["FollowersGeneratedCount"] = await _context.Campaigns.Where(m => m.Status == 1).SumAsync(m => m.TotalFollowerIncreament);
            ViewData["FollowersGeneratedCountThisMonth"] = await _context.Campaigns.Where(m => m.CreationDate <= toDate && m.CreationDate >= fromDate && m.Status == 1).SumAsync(m => m.TotalFollowerIncreament);

            double expenseTotal = _context.Posts.Where(m => m.Campaign.Status == 1).Sum(m => m.PaidAmount);
            expenseTotal += _context.Expenses.Sum(m => m.Amount);



            ViewData["TotalExpence"] = expenseTotal;



            ViewData["TotalIncome"]= await _context.Campaigns.Where(m => m.Status == 1).SumAsync(m => m.TotalBudget);



            return View();
        }

        public IActionResult AssignmentData(DateTime getDate, DateTime fromDate)
        {
            if (fromDate == default(DateTime))
            {
                fromDate = DateTime.Now.Date;
            }

            ViewData["Date"] = fromDate.Date;
            return View();
        }

      
        public IActionResult Privacy()
        {
            return View();
        }
      

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
