using System.ComponentModel.DataAnnotations;

namespace HeapProject.Models;

public class HeapHistoryItem
{
    public int Id { get; set; }
    public DateTime SavedTime { get; set; }
    public String? HeapItem { get; set; }
}
