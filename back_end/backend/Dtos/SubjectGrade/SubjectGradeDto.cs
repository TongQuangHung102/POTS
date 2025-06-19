using backend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Dtos.SubjectGrade
{
    public class SubjectGradeDto
    {
        public int SubjectId { get; set; }

        public int GradeId { get; set; }
    }
}
