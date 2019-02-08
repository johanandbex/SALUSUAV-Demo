using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SALUSUAV_Demo.Models;
using SALUSUAV_Demo.Models.UAVData;
using SALUSUAV_DEMO.Data;

namespace SALUSUAV_Demo.Controllers
{
    public class UavDatasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UavDatasController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: UAVDatas
        public async Task<IActionResult> Index()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View(await _context.UavData.Where(t => t.UserId == userId).ToListAsync());
        }

        // GET: UAVDatas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uAvData = await _context.UavData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (uAvData == null)
            {
                return NotFound();
            }

            return View(uAvData);
        }

        // GET: UAVDatas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UAVDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Manufacturer,Model,PurchaseYear,MaxWindSpeed,MaxAltitude,Weight,BeyondSightEnabled,SafetyMechanism")] UavData uAvData)
        {
            if (ModelState.IsValid)
            {
                uAvData.UserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _context.Add(uAvData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uAvData);
        }

        // GET: UAVDatas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uAvData = await _context.UavData.SingleOrDefaultAsync(m => m.Id == id);
            if (uAvData == null)
            {
                return NotFound();
            }
            return View(uAvData);
        }

        // POST: UAVDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Manufacturer,Model,PurchaseYear,MaxWindSpeed,MaxAltitude,Weight,BeyondSightEnabled,SafetyMechanism")] UavData uAvData)
        {
            if (id != uAvData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    uAvData.UserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _context.Update(uAvData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UavDataExists(uAvData.Id))
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
            return RedirectToAction(nameof(Index));
        }

        // GET: UAVDatas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uAvData = await _context.UavData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (uAvData == null)
            {
                return NotFound();
            }

            return View(uAvData);
        }

        // POST: UAVDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var uAvData = await _context.UavData.SingleOrDefaultAsync(m => m.Id == id);
            _context.UavData.Remove(uAvData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UavDataExists(Guid id)
        {
            return _context.UavData.Any(e => e.Id == id);
        }
    }
}
