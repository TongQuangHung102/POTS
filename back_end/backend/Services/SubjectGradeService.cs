using backend.Dtos.SubjectGrade;
using backend.Models;
using backend.Repositories;
using Newtonsoft.Json;

namespace backend.Services
{
    public class SubjectGradeService
    {
        private readonly ISubjectGradeRepository _subjectGradeRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SubjectGradeService(
            ISubjectGradeRepository subjectGradeRepository,
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment webHostEnvironment)
        {
            _subjectGradeRepository = subjectGradeRepository;
            _httpClientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<SubjectGrade>> GetAllSubjectByGradeAsync(int gradeId)
        {
            return await _subjectGradeRepository.GetAllSubjectByGradeAsync(gradeId);
        }

        public async Task<SubjectGrade> GetBySubjectGradeAsync(int gradeId, int subjectId)
        {
            var subjectGrade = await _subjectGradeRepository.GetByGradeAndSubjectAsync(gradeId, subjectId);
            if (subjectGrade == null)
            {
                throw new InvalidOperationException("Chưa có môn học nào ở khối này!");
            }
            return subjectGrade;
        }


        public async Task<SubjectGrade> GetTestBySubjectGradeAsync(int gradeId, int subjectId)
        {
            return await _subjectGradeRepository.GetTestByGradeAndSubjectAsync(gradeId, subjectId);
        }

        public async Task AddNewSubjectGrade(SubjectGradeDto subjectGradeDto, IFormFile fileKy1, IFormFile fileKy2)
        {
            var existingRecord = await _subjectGradeRepository.GetByGradeAndSubjectAsync(
                 subjectGradeDto.GradeId, subjectGradeDto.SubjectId);

            if (existingRecord != null)
            {
                throw new InvalidOperationException("Môn học đã tồn tại trong khối lớp này.");
            }

            string collectionName = "collection" + subjectGradeDto.GradeId.ToString() + subjectGradeDto.SubjectId.ToString() + "ky";

  
            string collectionName1 = await ProcessFileAndGetCollectionName(fileKy1, (collectionName + "1"));
            string collectionName2 = await ProcessFileAndGetCollectionName(fileKy2, (collectionName + "2"));


            if (string.IsNullOrEmpty(collectionName1) || string.IsNullOrEmpty(collectionName2))
            {
                throw new InvalidOperationException("Không nhận được collection_name từ API.");
            }


            var sg = new SubjectGrade
            {
                GradeId = subjectGradeDto.GradeId,
                SubjectId = subjectGradeDto.SubjectId,
                CollectionName = collectionName 
            };

            await _subjectGradeRepository.AddAsync(sg);

        }


        private async Task<string> ProcessFileAndGetCollectionName(IFormFile file, string fileName)
        {
            var allowedExtensions = new[] { ".pdf", ".xlsx", ".docx" }; 
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Định dạng file không hợp lệ. Vui lòng tải lên một file PDF, Excel hoặc Word.");
            }

            const long maxFileSize = 100 * 1024 * 1024;  // 100MB
            if (file.Length > maxFileSize)
            {
                throw new InvalidOperationException("Dung lượng file vượt quá giới hạn cho phép. Vui lòng tải lên file có dung lượng dưới 100MB.");
            }

            var newFileName = fileName + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var collectionName = await CallAIFileProcessingAPI(filePath);

            return collectionName;
        }


        private async Task<string> CallAIFileProcessingAPI(string filePath)
        {
            try
            {
                Console.WriteLine("📤 Bắt đầu gửi file đến AI: " + filePath);

                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(15)
                };

                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var content = new MultipartFormDataContent();
                content.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

                Console.WriteLine("📡 Gửi request POST đến AI Service...");

                var response = await client.PostAsync("http://localhost:5000/upload-file", content);
                //var response = await client.PostAsync("http://ai:5000/upload-file", content);

                Console.WriteLine("📥 Nhận phản hồi từ AI: " + response.StatusCode);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("📄 Nội dung phản hồi: " + responseString);

                var apiResponse = JsonConvert.DeserializeObject<AIFileProcessingResponseDto>(responseString);

                if (apiResponse == null || string.IsNullOrEmpty(apiResponse.CollectionName))
                {
                    Console.WriteLine("⚠️ Phản hồi từ AI không hợp lệ hoặc thiếu collection name.");
                    throw new Exception("Xử lý file với AI không thành công.");
                }

                Console.WriteLine("✅ Hoàn tất xử lý. Collection: " + apiResponse.CollectionName);
                return apiResponse.CollectionName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi khi gọi AI service xử lý file: " + ex.Message);
                throw;
            }
        }


    }
}
