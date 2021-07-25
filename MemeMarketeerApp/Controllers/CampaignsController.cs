using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MemeMarketeerApp.Data;
using MemeMarketeerApp.Models;
using MemeMarketeerApp.Models.ViewModels;

namespace MemeMarketeerApp.Controllers
{
    public class CampaignsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CampaignsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Campaigns
        public async Task<IActionResult> Index()
        {
            var allSoloCampaigns = await _context.Campaigns.OrderByDescending(m => m.Id).Include(c => c.Client).ToListAsync();

            List<CampaignHomeViewModel> campaigns = new List<CampaignHomeViewModel>();

            foreach (var soloCampaign in allSoloCampaigns)
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


            return View(campaigns);
        }

        // GET: Campaigns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var campaign = await _context.Campaigns
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (campaign == null)
            {
                return NotFound();
            }

            double totalImpressions = await _context.Posts.Where(m => m.Campaign.Id == id && m.Campaign.Status == 1).SumAsync(m => m.ImpressionGenerated);

            double? totalLinkClicks = await _context.Campaigns.Where(m => m.Id == id && m.Status == 1).SumAsync(m => m.TotalLinkClicked);
            double? totalFollowerGenerated = await _context.Campaigns.Where(m => m.Id == id && m.Status == 1).SumAsync(m => m.TotalFollowerIncreament);


            double expenseTotal = _context.Posts.Where(m => m.Campaign.Id == id && m.Campaign.Status == 1).Sum(m => m.PaidAmount);
            expenseTotal += _context.Expenses.Where(m => m.Campaign.Id == id && m.Campaign.Status == 1).Sum(m => m.Amount);



            double? totalProfit = campaign.TotalBudget - expenseTotal;

            var affiliates = await _context.Posts
            .Where(m => m.CampaignId == id)
             .Select(o => o.Affiliate).Distinct()
             .Include(m=>m.Catagory)
             .Include(m=>m.AffiliateLanguage)
            .ToListAsync();




            ViewData["TotalImpression"] = totalImpressions;
            ViewData["TotalLinkClick"] = totalLinkClicks;
            ViewData["TotalFollowersGenerated"] = totalFollowerGenerated;
            ViewData["TotalExpense"] = expenseTotal;
            ViewData["TotalProfit"] = totalProfit;
            ViewData["Campaign"] = campaign;
            ViewData["Affiliates"] = affiliates;
            return View(campaign);
        }

        // GET: Campaigns/Create
        public IActionResult Create(int fromHome)
        {
            ViewData["FromHome"] = fromHome;
            ViewData["ClientId"] = new SelectList(_context.Clients.Where(m => m.Status == 1), "Id", "Name");
            return View();
        }

        public IActionResult AddActivity(int? id, int fromHome)
        {
            if (id == null)
            {
                return NotFound();
            }

            var campaign = _context.Campaigns.Where(m => m.Id == id)
                .Include(m => m.Client)
                .Include(m => m.Posts)
                .ThenInclude(m => m.PostType)
                .Include(m => m.Posts)
                .ThenInclude(m => m.Affiliate)
                .FirstOrDefault();
            campaign.FromHome = fromHome;
            if (campaign == null)
            {
                return NotFound();
            }
            ViewData["AffiliateId"] = _context.Affiliates.Where(m => m.Status == 1).ToList();
            ViewData["PostTypeId"] = _context.PostTypes.ToList();



            return View(campaign);
        }

        [HttpPost]
        public async Task<JsonResult> AddPost(int affiliateId, int postTypeId, string postTitle, double impressionGenerated, double postReach, double storyViewed, string reactionGenerated, string commentGenerated, double paidAmount, int postId, int campaignId)

        {
            if (postId == 0)
            {
                Post post = new Post();
                post.AffiliateId = affiliateId;
                post.PostTypeId = postTypeId;
                post.Name = postTitle;
                post.ImpressionGenerated = impressionGenerated;
                post.PostReached = postReach;
                post.StoryViewed = storyViewed;
                post.ReactionGenerated = reactionGenerated;
                post.CommentsGenerated = commentGenerated;
                post.PaidAmount = paidAmount;
                post.CampaignId = campaignId;
                _context.Add(post);
                await _context.SaveChangesAsync();

            }

            else
            {
                var post = await _context.Posts.Where(m => m.Id == postId).FirstOrDefaultAsync();

                post.AffiliateId = affiliateId;
                post.PostTypeId = postTypeId;
                post.Name = postTitle;
                post.ImpressionGenerated = impressionGenerated;
                post.PostReached = postReach;
                post.StoryViewed = storyViewed;
                post.ReactionGenerated = reactionGenerated;
                post.CommentsGenerated = commentGenerated;
                post.PaidAmount = paidAmount;
                post.CampaignId = campaignId;
                _context.Update(post);
                await _context.SaveChangesAsync();

            }
            return Json(true);
        }
        public async Task<JsonResult> FinalizedCampaign(int campaignId, double generatedFollowers, double generatedLinkClicks,string bitlyLink, int fromHome)
        {
            var campaign = await _context.Campaigns.Where(m => m.Id == campaignId).FirstOrDefaultAsync();
            if (campaign != null)
            {
                campaign.BitlyLink = bitlyLink;
                campaign.TotalFollowerIncreament = generatedFollowers;
                campaign.TotalLinkClicked = generatedLinkClicks;
                campaign.Status = 1;
                _context.Update(campaign);
                await _context.SaveChangesAsync();
                return Json(fromHome);
            }
            else
            {
                return Json(false);
            }
        }



        // POST: Campaigns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Campaign campaign)
        {
            campaign.Status = 0;
            campaign.CreationDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(campaign);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients.Where(m => m.Status == 1), "Id", "Name", campaign.ClientId);
            return View(campaign);
        }

        // GET: Campaigns/Edit/5
        public async Task<IActionResult> Edit(int? id, int fromHome)
        {
            if (id == null)
            {
                return NotFound();
            }

            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null)
            {
                return NotFound();
            }
            campaign.FromHome = fromHome;
            ViewData["ClientId"] = new SelectList(_context.Clients.Where(m => m.Status == 1 || m.Id == campaign.ClientId), "Id", "Name", campaign.ClientId);
            return View(campaign);
        }

        // POST: Campaigns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Campaign campaign)
        {
            if (id != campaign.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(campaign);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CampaignExists(campaign.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (campaign.FromHome == 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewData["ClientId"] = new SelectList(_context.Clients.Where(m => m.Status == 1 || m.Id == campaign.ClientId), "Id", "Name", campaign.ClientId);
            return View(campaign);
        }



        private bool CampaignExists(int id)
        {
            return _context.Campaigns.Any(e => e.Id == id);
        }

        // GET: Campaigns/Create
        public async Task<IActionResult> ClientCampaign(int clientId)
        {


            var client = await _context.Clients.Where(m => m.Id == clientId).FirstOrDefaultAsync();

            var campaigns = await _context.Campaigns.Where(m => m.ClientId == clientId && m.Status == 1).ToListAsync();

            List<ClientCampaign> clientCampaigns = new List<ClientCampaign>();

            foreach (var campaign in campaigns)
            {
                ClientCampaign clientCampaign = new ClientCampaign();
                clientCampaign.Campaign = campaign;
                var affiliates = await _context.Posts
    
                    .Where(m => m.CampaignId == campaign.Id)
                    .Select(o => o.Affiliate).Distinct()
                    .Include(m => m.AffiliateLanguage)
                    .Include(m => m.Catagory)
                    .ToListAsync();

                clientCampaign.Affiliates = affiliates;
                clientCampaigns.Add(clientCampaign);

            }



            ViewData["ClientCampaigns"] = clientCampaigns;
            ViewData["Client"] = client;
            return View();
        }

        
        // GET: Campaigns/Create
        public async Task<IActionResult> CapmaignReport(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var campaign = await _context.Campaigns.Where(m => m.Id == id)
                .Include(m=>m.Client)
                .Include(m=>m.Posts)
                .ThenInclude(m=>m.Affiliate)
                .Include(m=>m.Posts)
                .ThenInclude(m=>m.PostType)
                .FirstOrDefaultAsync();
         

            double generatedImpression = campaign.Posts.Sum(m => m.ImpressionGenerated);

            double costPer1000Impression = campaign.TotalBudget/(generatedImpression / 1000) ;
            double costPerFollower=(double) (campaign.TotalBudget/campaign.TotalFollowerIncreament);
            double costPerClick = 0;
            if (campaign.TotalLinkClicked!=0)
            {
                costPerClick= (double)(campaign.TotalBudget / campaign.TotalLinkClicked);
            }

            ViewData["Campaign"] = campaign;
            ViewData["CostPer1000Impression"] = costPer1000Impression;
            ViewData["GeneratedImpression"] = generatedImpression;
            ViewData["CostPerFollwer"] = costPerFollower;
            ViewData["CostPerClick"] = costPerClick;

            return View();
        }

        public async Task<IActionResult> CapmaignReportWithImpression(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var campaign = await _context.Campaigns.Where(m => m.Id == id)
                .Include(m=>m.Client)
                .Include(m=>m.Posts)
                .ThenInclude(m=>m.Affiliate)
                .Include(m=>m.Posts)
                .ThenInclude(m=>m.PostType)
                .FirstOrDefaultAsync();
         

            double generatedImpression = campaign.Posts.Sum(m => m.ImpressionGenerated);

            double costPer1000Impression = campaign.TotalBudget/(generatedImpression / 1000) ;
            double costPerFollower=(double) (campaign.TotalBudget/campaign.TotalFollowerIncreament);
            double costPerClick = 0;
            if (campaign.TotalLinkClicked!=0)
            {
                costPerClick= (double)(campaign.TotalBudget / campaign.TotalLinkClicked);
            }

            ViewData["Campaign"] = campaign;
            ViewData["CostPer1000Impression"] = costPer1000Impression;
            ViewData["GeneratedImpression"] = generatedImpression;
            ViewData["CostPerFollwer"] = costPerFollower;
            ViewData["CostPerClick"] = costPerClick;

            return View();
        }

        public async Task<IActionResult> CapmaignReportNoDetails(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }
            var campaign = await _context.Campaigns.Where(m => m.Id == id)
                .Include(m=>m.Client)
                .Include(m=>m.Posts)
                .ThenInclude(m=>m.Affiliate)
                .Include(m=>m.Posts)
                .ThenInclude(m=>m.PostType)
                .FirstOrDefaultAsync();
         

            double generatedImpression = campaign.Posts.Sum(m => m.ImpressionGenerated);

            double costPer1000Impression = campaign.TotalBudget/(generatedImpression / 1000) ;
            double costPerFollower=(double) (campaign.TotalBudget/campaign.TotalFollowerIncreament);
            double costPerClick = 0;
            if (campaign.TotalLinkClicked!=0)
            {
                costPerClick= (double)(campaign.TotalBudget / campaign.TotalLinkClicked);
            }

            ViewData["Campaign"] = campaign;
            ViewData["CostPer1000Impression"] = costPer1000Impression;
            ViewData["GeneratedImpression"] = generatedImpression;
            ViewData["CostPerFollwer"] = costPerFollower;
            ViewData["CostPerClick"] = costPerClick;

            return View();
        }


    }
}
