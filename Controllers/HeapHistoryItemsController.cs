
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeapProject.Models;
using System.Text.Json.Nodes;
using System.IO;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;


public class HeapHistoryItemsController : Controller
{
    private readonly HeapProjectContext _context;
    private readonly int MAX_SIZE_HEAP = 15;


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
    public async Task<IActionResult> Create([Bind("Id,SavedTime,HeapItem,HeapItemLabel")] HeapHistoryItem heaphistoryitem)
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

    public class HeapParsedItem
    {
        public int key { get; set; }
        public string value { get; set; }
        public HeapParsedItem Parent { get; set; }
        public float cx { get; set; }
        public float cy { get; set; }
        public int iteration { get; set; }
    }

    public class HeapHistoryParsedItem
    {
        public HeapParsedItem[] heapArray { get; set; }
        public int[] stackOfTxtNodes { get; set; }
        public int count { get; set; }
    }

    private bool AreFieldsValid(HeapHistoryParsedItem item)
    {
        if(item.count == SizeOfHeapParsedItemsArray(item.heapArray) && 
           item.count == item.stackOfTxtNodes.Length && 
           item.count <= MAX_SIZE_HEAP)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int SizeOfHeapParsedItemsArray(HeapParsedItem[] items)
    {
        int count = 0;
        while (count < items.Length && items[count] != null)
        {
            count++;
        }
        return count;
    }


    // count for stack (separate size method?)

    public bool IsValidMaxHeap(string? heapHistoryItem)
    {
        HeapHistoryParsedItem? item = JsonSerializer.Deserialize<HeapHistoryParsedItem>(heapHistoryItem);

        //JsonNode node = JsonNode.Parse(heapHistoryItem);
        //System.IO.File.WriteAllText("c:\\test\\test5-two-children-all-nodes-used.txt", node.ToString());
 
        // have to check that stack contains as many non-null nodes as are in the array and that the count matches

        bool result = false; 
        if (item != null && item.heapArray != null && item.heapArray.Length > 0     // valid heap with > 1 element
            && item.heapArray[0] != null && AreFieldsValid(item))
        {
            result = AreChildrenLessAndNoDuplicates(item.heapArray);
        }
        else if(item != null && item.heapArray != null && item.heapArray.Length > 0 
                && item.heapArray[0] == null && AreFieldsValid(item))                // empty heap
        {
            result = true;      // empty heap (check for stack and count to match)
        }        
        //else if(item != null && item.heapArray != null && item.heapArray.Length )

        
        return result;
    }

    private bool AreChildrenLessAndNoDuplicates(HeapParsedItem[] heapArray)
    {
        Queue<HeapParsedItem> heapParsedItems = new Queue<HeapParsedItem>();
        Queue<int> heapIndices = new Queue<int>();
        HashSet<int> keys = new HashSet<int>();

        heapParsedItems.Enqueue(heapArray[0]);
        heapIndices.Enqueue(0);
        keys.Add(heapArray[0].key);

        while(heapParsedItems.Count > 0) 
        {
            HeapParsedItem item = heapParsedItems.Dequeue();
            int index = heapIndices.Dequeue();
            int leftChildIndex = 2 * index + 1;
            int rightChildIndex = 2 * index + 2;
            if(leftChildIndex < heapArray.Length && heapArray[leftChildIndex] != null)
            {
                if (heapArray[leftChildIndex].key < heapArray[index].key && keys.Add(heapArray[leftChildIndex].key))
                {
                    heapParsedItems.Enqueue(heapArray[leftChildIndex]);
                    heapIndices.Enqueue(leftChildIndex);
                }
                else
                {
                    return false;
                }
            }
            if(rightChildIndex < heapArray.Length && heapArray[rightChildIndex] != null)
            {
                if (heapArray[rightChildIndex].key < heapArray[index].key && keys.Add(heapArray[rightChildIndex].key))
                {
                    heapParsedItems.Enqueue(heapArray[rightChildIndex]);
                    heapIndices.Enqueue(rightChildIndex);
                }
                else
                {
                    return false;
                }
            }
        }

        return true;
    }

    // POST: HEAPHISTORYITEMS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,SavedTime,HeapItem,HeapItemLabel")] HeapHistoryItem heaphistoryitem)
    {
        if (id != heaphistoryitem.Id)
        {
            return NotFound();
        }

        //JsonNode node = JsonNode.Parse(heaphistoryitem.HeapItem);
        //System.IO.File.WriteAllText("c:\\test\\test1-emptyheap.txt", node.ToString());

        if (!IsValidMaxHeap(heaphistoryitem.HeapItem))
        {
            return BadRequest();    // TODO: response good enough for now
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(heaphistoryitem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HeapHistoryItemExists(heaphistoryitem.Id))
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
        return View(heaphistoryitem);
    }

    //public class HeapHistoryData
    //{
    //    public string? Data { get; set; }
    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public IActionResult SaveHeap([FromBody] HeapHistoryData heapHistoryData)
    //{
    //    if (heapHistoryData == null) return BadRequest("Invalid data");
        
    //    // save to DB, then do a select for the information, and return it, and make sure it matches the (simply)
    //    // reflected data (make sure it was saved to DB correctly)
        
    //    return Json(new { success = true, message = $"{heapHistoryData.Data}" });
    //}

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
