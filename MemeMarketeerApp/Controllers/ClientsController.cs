using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MemeMarketeerApp.Data;
using MemeMarketeerApp.Models;

namespace MemeMarketeerApp.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.OrderByDescending(m => m.Id).ToListAsync());
        }

        // GET: Affiliates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);

            int numberOfCampaign = await _context.Campaigns.Where(m => m.ClientId == id &&m.Status==1).CountAsync();
            double totalImpressions = await _context.Posts.Where(m => m.Campaign.ClientId == id && m.Campaign.Status == 1).SumAsync(m => m.ImpressionGenerated);
            double? totalLinkClicks = await _context.Campaigns.Where(m => m.ClientId == id && m.Status == 1).SumAsync(m => m.TotalLinkClicked);
            double? totalFollowerGenerated = await _context.Campaigns.Where(m => m.ClientId == id && m.Status == 1).SumAsync(m => m.TotalFollowerIncreament);
            double? totalIncome = await _context.Campaigns.Where(m => m.ClientId == id && m.Status == 1).SumAsync(m => m.TotalBudget);

            double expenseTotal = _context.Posts.Where(m => m.Campaign.ClientId == id && m.Campaign.Status == 1).Sum(m => m.PaidAmount);
            expenseTotal += _context.Expenses.Where(m => m.Campaign.ClientId == id && m.Campaign.Status == 1).Sum(m => m.Amount);


            double? totalProfit = totalIncome- expenseTotal;

            ViewData["NumberOfCampaign"] = numberOfCampaign;
            ViewData["TotalImpression"] = totalImpressions;
            ViewData["TotalLinkClick"] = totalLinkClicks;
            ViewData["TotalFollowersGenerated"] = totalFollowerGenerated;
            ViewData["TotalIncome"] = totalIncome;
            ViewData["TotalExpense"] = expenseTotal;
            ViewData["TotalProfit"] = totalProfit;
            ViewData["Client"] = client;


            if (client == null)
            {
                return NotFound();
            }

            return View();
        }

        // GET: Affiliates/Create
        public IActionResult Create(int fromHome)
        {
            ViewData["FromHome"] = fromHome;
            return View();
        }

        // POST: Affiliates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            client.Status = 0;
            client.AddintionDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                if (client.FromHome == 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            return View(client);
        }

        // GET: Affiliates/Edit/5
        public async Task<IActionResult> Edit(int? id, int fromHome)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            client.FromHome = fromHome;
            return View(client);
        }

        // POST: Affiliates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (client.FromHome == 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            return View(client);
        }

      

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Enable(int? id, int fromHome)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }
            else
            {
                client.Status = 1;
                _context.Update(client);
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

            var client = await _context.Clients
                 .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }
            else
            {
                client.Status = 0;
                _context.Update(client);
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
