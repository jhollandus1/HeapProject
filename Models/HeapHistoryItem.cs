namespace HeapProject.Models;

using System.ComponentModel.DataAnnotations;
public class HeapHistoryItem
{
    public int Id { get; set; }

    [Display(Name = "Saved Time")]
    public DateTime SavedTime { get; set; }
    
    public String? HeapItem { get; set; }
    
    [Display(Name = "Heap item label")]
    public required String HeapItemLabel { get; set; }

}
