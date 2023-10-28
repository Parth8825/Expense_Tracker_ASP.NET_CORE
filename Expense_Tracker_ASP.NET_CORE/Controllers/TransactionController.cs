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
using System.Security.Claims;

namespace Expense_Tracker_ASP.NET_CORE.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public TransactionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(this.User);
            var applicationDbContext = _context.Transactions
                .Where(x => x.UserId == userId)
                .Include(t => t.Category);
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id=0)
        {
            PopulateCategories();
            if(id == 0)
            {
                var newTransaction = new Transaction();
                // Get the currently logged-in user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                newTransaction.UserId = userId;
                return View(new Transaction());
            } 
            else
            {
                var existingTransaction = _context.Transactions.Find(id);
                if (existingTransaction == null)
                {
                    return NotFound();
                }
                return View(existingTransaction);
            }
        }

        // POST: Transaction/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(this.User);

                if (transaction.TransactionId == 0)
                {
                    transaction.UserId = userId;
                    _context.Add(transaction);
                }
                else
                {
                    var existingTransaction = _context.Transactions.Find(transaction.TransactionId);
                    if(existingTransaction == null)
                    {
                        return NotFound();
                    }
                    if(existingTransaction.UserId != userId)
                    {
                        return Forbid();
                    }

                    existingTransaction.CategoryId = transaction.CategoryId;
                    existingTransaction.Amount = transaction.Amount;
                    existingTransaction.Note = transaction.Note;
                    existingTransaction.Date = transaction.Date;

                    _context.Update(existingTransaction);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateCategories();
            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void PopulateCategories()
        {
            var CategoryCollection = _context.Categories.ToList();
            Category DefaultCategory = new Category() { CategoryId = 0, Title = "Choose a Category" };
            CategoryCollection.Insert(0,DefaultCategory);
            ViewBag.Categories = CategoryCollection;
        }
    }
}
