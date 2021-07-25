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
    public class AffiliatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AffiliatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Affiliates
        public async Task<IActionResult> Index()
        {
            return View(await _context.Affiliates.Include(m => m.AffiliateLanguage)
                .Include(m => m.Catagory).OrderByDescending(m=>m.Id).ToListAsync());
        }

        // GET: Affiliates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var affiliate = await _context.Affiliates
                .Include(m=>m.AffiliateLanguage)
                .Include(m=>m.Catagory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (affiliate == null)
            {
                return NotFound();
            }
            double totalPostReach = await _context.Posts.Where(m => m.AffiliateId == id && m.Campaign.Status == 1 && m.PostReached != 0).SumAsync(m => m.PostReached);

             double totalPost = await _context.Posts.Where(m => m.AffiliateId == id && m.Campaign.Status == 1 && m.PostReached!=0).CountAsync();


             double totalStoryView = await _context.Posts.Where(m => m.AffiliateId == id && m.Campaign.Status == 1 && m.StoryViewed != 0).SumAsync(m => m.StoryViewed);

             double totalStory = await _context.Posts.Where(m => m.AffiliateId == id && m.Campaign.Status == 1 && m.StoryViewed != 0).CountAsync();

            double averagePostReach = 0;
            double averageStoryView = 0;
            if (totalPostReach!=0 && totalPost!=0)
            {
                averagePostReach = totalPostReach / totalPost;
            }
            if (totalStoryView != 0 && totalStory != 0)
            {
                averageStoryView = totalStoryView / totalStory;
            }
            
           

            
            double totalPaid = _context.Posts.Where(m => m.AffiliateId == id && m.Campaign.Status == 1).Sum(m => m.PaidAmount);
            

            var soloCampaigns = await _context.Posts
            .Where(m => m.AffiliateId == id)
             .Select(o => o.Campaign).Distinct()
             .Include(m=>m.Client)
            .ToListAsync();

            List<CampaignHomeViewModel> campaigns = new List<CampaignHomeViewModel>();

            foreach (var soloCampaign in soloCampaigns)
            {
                CampaignHomeViewModel campaign = new CampaignHomeViewModel();
                campaign.Campaign = soloCampaign;
                if (soloCampaign.Status == 1)
                {
                    campaign.TotalImpressionGenerated = Campaign.GetImpressionsGenerated(soloCampaign.Id, _context);

                }

                campaigns.Add(campaign);
            }



            ViewData["AveragePostReach"] = averagePostReach;
            ViewData["AverageStoryView"] = averageStoryView;
            ViewData["TotalPaid"] = totalPaid;
            ViewData["Campaigns"] = campaigns;
            ViewData["Affiliate"] = affiliate;
            return View();
        }

        // GET: Affiliates/Create
        public IActionResult Create(int fromHome)
        {
            ViewData["AffiliateLanguageId"] = new SelectList(_context.AffiliateLanguages.Where(m => m.Status == 1), "Id", "Name");
            ViewData["CatagoryId"] = new SelectList(_context.Catagories.Where(m => m.Status == 1), "Id", "Name");
            ViewData["FromHome"] = fromHome;
            return View();
        }

        // POST: Affiliates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Affiliate affiliate)
        {
            affiliate.Status = 0;
            affiliate.AddintionDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(affiliate);
                await _context.SaveChangesAsync();

                if (affiliate.FromHome == 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                
            }
            return View(affiliate);
        }

        // GET: Affiliates/Edit/5
        public async Task<IActionResult> Edit(int? id,int fromHome)
        {
            if (id == null)
            {
                return NotFound();
            }

            var affiliate = await _context.Affiliates.FindAsync(id);
            if (affiliate == null)
            {
                return NotFound();
            }
          
            ViewData["AffiliateLanguageId"] = new SelectList(_context.AffiliateLanguages.Where(m => m.Status == 1 ||m.Id==affiliate.AffiliateLanguageId), "Id", "Name", affiliate.AffiliateLanguageId);
                ViewData["CatagoryId"] = new SelectList(_context.Catagories.Where(m => m.Status == 1 ||m.Id==affiliate.CatagoryId), "Id", "Name", affiliate.CatagoryId);
            
            affiliate.FromHome = fromHome;
            return View(affiliate);
        }

        // POST: Affiliates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  Affiliate affiliate)
        {
            if (id != affiliate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(affiliate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AffiliateExists(affiliate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (affiliate.FromHome==0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                
            }
            ViewData["AffiliateLanguageId"] = new SelectList(_context.AffiliateLanguages.Where(m => m.Status == 1 || m.Id == affiliate.AffiliateLanguageId), "Id", "Name", affiliate.AffiliateLanguageId);
            ViewData["CatagoryId"] = new SelectList(_context.Catagories.Where(m => m.Status == 1 || m.Id == affiliate.CatagoryId), "Id", "Name", affiliate.CatagoryId);

            return View(affiliate);
        }

        
        private bool AffiliateExists(int id)
        {
            return _context.Affiliates.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Enable(int? id, int fromHome)
        {
            if (id == null)
            {
                return NotFound();
            }

            var affiliate = await _context.Affiliates
                .FirstOrDefaultAsync(m => m.Id == id);
            if (affiliate == null)
            {
                return NotFound();
            }
            else
            {
                affiliate.Status = 1;
                _context.Update(affiliate);
                await _context.SaveChangesAsync();

                if (fromHome == 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

        }
        public async Task<IActionResult> Disable(int? id, int fromHome)
        {
            if (id == null)
            {
                return NotFound();
            }

            var affiliate = await _context.Affiliates
                 .FirstOrDefaultAsync(m => m.Id == id);
            if (affiliate == null)
            {
                return NotFound();
            }
            else
            {
                affiliate.Status = 0;
                _context.Update(affiliate);
                await _context.SaveChangesAsync();

                if (fromHome == 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

        }



    }
}
