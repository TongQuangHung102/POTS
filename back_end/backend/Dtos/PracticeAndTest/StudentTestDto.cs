using backend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Dtos.PracticeAndTest
{
    public class StudentTestDto
    {
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double? Score { get; set; }
        public int TestId { get; set; }
        public int UserId { get; set; }
    }
}
