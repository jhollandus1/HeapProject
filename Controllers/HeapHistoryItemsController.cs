
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeapProject.Models;

public class HeapHistoryItemsController : Controller
{
    private readonly HeapProjectContext _context;

    public HeapHistoryItemsController(HeapProjectContext context)
    {
        _context = context;
    }

    // GET: HEAPHISTORYITEMS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.HeapHistoryItem.ToListAsync());
    }

    // GET: HEAPHISTORYITEMS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var heaphistoryitem = await _context.HeapHistoryItem
            .FirstOrDefaultAsync(m => m.Id == id);
        if (heaphistoryitem == null)
        {
            return NotFound();
        }

        return View(heaphistoryitem);
    }

    // GET: HEAPHISTORYITEMS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: HEAPHISTORYITEMS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,SavedTime,HeapItem")] HeapHistoryItem heaphistoryitem)
    {
        if (ModelState.IsValid)
        {
            _context.Add(heaphistoryitem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(heaphistoryitem);
    }

    // GET: HEAPHISTORYITEMS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var heaphistoryitem = await _context.HeapHistoryItem.FindAsync(id);
        if (heaphistoryitem == null)
        {
            return NotFound();
        }
        return View(heaphistoryitem);
    }

    // POST: HEAPHISTORYITEMS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int? id, [Bind("Id,SavedTime,HeapItem")] HeapHistoryItem heaphistoryitem)
    //{
    //    if (id != heaphistoryitem.Id)
    //    {
    //        return NotFound();
    //    }

    //    if (ModelState.IsValid)
    //    {
    //        try
    //        {
    //            _context.Update(heaphistoryitem);
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!HeapHistoryItemExists(heaphistoryitem.Id))
    //            {
    //                return NotFound();
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }
    //        return RedirectToAction(nameof(Index));
    //    }
    //    return View(heaphistoryitem);
    //}

    public class HeapHistoryItem
    {
        public string? Data { get; set; }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SaveHeap([FromBody] HeapHistoryItem heapHistoryItem)
    {
        if (heapHistoryItem == null) return BadRequest("Invalid data");

        return Json(new { success = true, message = $"Received {heapHistoryItem.Data}" });
    }

    // GET: HEAPHISTORYITEMS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var heaphistoryitem = await _context.HeapHistoryItem
            .FirstOrDefaultAsync(m => m.Id == id);
        if (heaphistoryitem == null)
        {
            return NotFound();
        }

        return View(heaphistoryitem);
    }

    // POST: HEAPHISTORYITEMS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var heaphistoryitem = await _context.HeapHistoryItem.FindAsync(id);
        if (heaphistoryitem != null)
        {
            _context.HeapHistoryItem.Remove(heaphistoryitem);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool HeapHistoryItemExists(int? id)
    {
        return _context.HeapHistoryItem.Any(e => e.Id == id);
    }
}
