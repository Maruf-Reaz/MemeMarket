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
    public class AffiliateLanguagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AffiliateLanguagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AffiliateLanguages
        public async Task<IActionResult> Index()
        {
            return View(await _context.AffiliateLanguages.OrderByDescending(m => m.Id).ToListAsync());
        }

       

        // GET: AffiliateLanguages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AffiliateLanguages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AffiliateLanguage affiliateLanguage)
        {
            affiliateLanguage.Status = 0;
            if (ModelState.IsValid)
            {
                _context.Add(affiliateLanguage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(affiliateLanguage);
        }

        // GET: AffiliateLanguages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var affiliateLanguage = await _context.AffiliateLanguages.FindAsync(id);
            if (affiliateLanguage == null)
            {
                return NotFound();
            }
            return View(affiliateLanguage);
        }

        // POST: AffiliateLanguages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AffiliateLanguage affiliateLanguage)
        {
            if (id != affiliateLanguage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(affiliateLanguage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AffiliateLanguageExists(affiliateLanguage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(affiliateLanguage);
        }

        

        private bool AffiliateLanguageExists(int id)
        {
            return _context.AffiliateLanguages.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Enable(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var affiliateLanguage = await _context.AffiliateLanguages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (affiliateLanguage == null)
            {
                return NotFound();
            }
            else
            {
                affiliateLanguage.Status = 1;
                _context.Update(affiliateLanguage);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));

            }

        }
        public async Task<IActionResult> Disable(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var affiliateLanguage = await _context.AffiliateLanguages
                 .FirstOrDefaultAsync(m => m.Id == id);
            if (affiliateLanguage == null)
            {
                return NotFound();
            }
            else
            {
                affiliateLanguage.Status = 0;
                _context.Update(affiliateLanguage);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

        }


    }
}
