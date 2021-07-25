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
    public class CatagoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatagoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Catagories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Catagories.OrderByDescending(m => m.Id).ToListAsync());
        }

      

        // GET: Catagories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Catagories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Catagory catagory)
        {
            catagory.Status = 0;
            if (ModelState.IsValid)
            {
                _context.Add(catagory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(catagory);
        }

        // GET: Catagories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catagory = await _context.Catagories.FindAsync(id);
            if (catagory == null)
            {
                return NotFound();
            }
            return View(catagory);
        }

        // POST: Catagories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Catagory catagory)
        {
            if (id != catagory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catagory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatagoryExists(catagory.Id))
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
            return View(catagory);
        }

       

        private bool CatagoryExists(int id)
        {
            return _context.Catagories.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Enable(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catagory = await _context.Catagories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (catagory == null)
            {
                return NotFound();
            }
            else
            {
                catagory.Status = 1;
                _context.Update(catagory);
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

            var catagory = await _context.Catagories
                 .FirstOrDefaultAsync(m => m.Id == id);
            if (catagory == null)
            {
                return NotFound();
            }
            else
            {
                catagory.Status = 0;
                _context.Update(catagory);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

        }

    }
}
