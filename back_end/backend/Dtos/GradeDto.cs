using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Dtos
{
    public class GradeDto
    {

        public string GradeName { get; set; }
        public string Description { get; set; }
        public bool IsVisible { get; set; }

        public int UserId { get; set; }
    }
}
