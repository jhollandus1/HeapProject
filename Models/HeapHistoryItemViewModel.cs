using System.ComponentModel.DataAnnotations;
namespace HeapProject.Models
{
    public class HeapHistoryItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "SavedTime")]
        public string? SavedTimeString { get; set; }

        [Display(Name = "Encrypted item")]
        public string? EncryptedItem { get; set; }

        [Display(Name = "Encrypted item label")]
        public required string EncryptedItemLabel { get; set; } = string.Empty;
    }
}

