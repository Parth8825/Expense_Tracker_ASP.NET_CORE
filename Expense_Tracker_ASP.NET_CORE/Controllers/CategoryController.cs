using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker_ASP.NET_CORE.Models;
using Microsoft.AspNetCore.Authorization;
using Expense_Tracker_ASP.NET_CORE.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Expense_Tracker_ASP.NET_CORE.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public CategoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {

            var userId = _userManager.GetUserId(this.User);
              return _context.Categories != null ? 
                          View(await _context.Categories
                          .Where(i => i.UserId == userId)
                          .ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
        }


        // GET: Category/AddOrEdit
        public IActionResult AddOrEdit(int id=0)
        {
            if(id == 0)
            {
                var newCategory = new Category();
                // Get the currently logged-in user's ID
                var userId = _userManager.GetUserId(this.User);
                newCategory.UserId = userId;

                return View(new Category());
            }
            else
            {
                var existingCategory = _context.Categories.Find(id);
                if (existingCategory == null)
                {
                    return NotFound();
                }
                return View(existingCategory);
            }

        }

        // POST: Category/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("CategoryId,Title,Icon,Type")] Category category)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(this.User);
                if (category.CategoryId == 0)
                {
                    category.UserId = userId;
                    _context.Add(category);
                }
                else
                {
                    var existingCategory = _context.Categories.Find(category.CategoryId);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }
                    if (existingCategory.UserId != userId)
                    {
                        return Forbid(); // Handle unauthorized access to edit category
                    }
                    existingCategory.Title = category.Title;
                    existingCategory.Icon = category.Icon;
                    existingCategory.Type = category.Type;

                    _context.Update(existingCategory);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }



        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
