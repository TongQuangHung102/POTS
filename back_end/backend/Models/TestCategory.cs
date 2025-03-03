using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class TestCategory
    {
        [Key]
        public int TestCategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsVisible { get; set; }
    }
}
