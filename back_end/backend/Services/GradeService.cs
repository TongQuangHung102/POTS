using backend.Dtos;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services
{
    public class GradeService
    {
        private readonly IGradeRepository _gradeRepository;

        public GradeService(IGradeRepository gradeRepository)
        {
            _gradeRepository = gradeRepository;
        }

        public async Task<IActionResult> GetAllGradesAsync()
        {
            try
            {
                var grades = await _gradeRepository.GetAllGradesAsync();
                if (grades.Count == 0)
                {
                    return new OkObjectResult(new { Message = "Không có grade nào trong hệ thống." });
                }

                return new OkObjectResult(grades.Select(g => new
                {
                    gradeId = g.GradeId,
                    gradeName = g.GradeName,
                    gradeDescription = g.Description,
                    gradeIsVisible = g.IsVisible
                }));
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Message = "Đã xảy ra lỗi khi lấy danh sách grade.", Error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }
        public async Task<IActionResult> GetGradeByIdAsync(int id)
        {
            try
            {
                var grade = await _gradeRepository.GetGradeByIdAsync(id);
                if (grade == null)
                {
                    return new NotFoundObjectResult(new { Message = "Grade không tồn tại." });
                }

                return new OkObjectResult(new
                {
                    gradeId = grade.GradeId,
                    gradeName = grade.GradeName,
                    gradeDescription = grade.Description,
                    gradeIsVisible = grade.IsVisible
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Message = "Lỗi khi lấy grade.", Error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }
        public async Task<IActionResult> UpdateGradeAsync(int gradeId, GradeDto gradeDto)
        {
            try
            {
                var existingGrade = await _gradeRepository.GetGradeByIdAsync(gradeId);
                if (existingGrade == null)
                {
                    return new NotFoundObjectResult(new { Message = "Grade không tồn tại." });
                }

                existingGrade.GradeName = gradeDto.GradeName;
                existingGrade.Description = gradeDto.Description;
                existingGrade.IsVisible = gradeDto.IsVisible;

      
                await _gradeRepository.UpdateGradeAsync(existingGrade);

                return new OkObjectResult(new { Message = "Cập nhật grade thành công!" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Message = "Lỗi khi cập nhật grade.", Error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }
        public async Task<IActionResult> AddGradeAsync(GradeDto gradeDto)
        {
            try
            {
                var newGrade = new Grades
                {
                    GradeName = gradeDto.GradeName,
                    Description = gradeDto.Description,
                    IsVisible = gradeDto.IsVisible
                };

                await _gradeRepository.AddGradeAsync(newGrade); 

                return new OkObjectResult(new
                {
                    Message = "Thêm grade thành công!"
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Message = "Lỗi khi thêm grade.", Error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }



    }
}
