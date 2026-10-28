using System.ComponentModel.DataAnnotations;
namespace HeapProject.Models
{
    public class HeapHistoryItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "SavedTime")]
        public string? SavedTimeString { get; set; }

        [Display(Name = "HeapItem")]
        public string? HeapItem { get; set; }

        [Display(Name = "HeapItemLabel")]
        public required string HeapItemLabel { get; set; } = string.Empty;
    }
}

