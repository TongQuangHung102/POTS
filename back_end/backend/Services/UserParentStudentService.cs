using backend.DataAccess.DAO;
using backend.Dtos.Users;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services
{
    public class UserParentStudentService
    {
        private readonly IUserParentStudentRepository _userParentStudentRepository;
        private readonly IUserRepository _userRepository;
        private readonly SendMailService _sendMailService;

        public UserParentStudentService(IUserParentStudentRepository userParentStudentRepository, IUserRepository userRepository, SendMailService sendMailService)
        {
            _userParentStudentRepository = userParentStudentRepository;
            _userRepository = userRepository;
            _sendMailService = sendMailService;
        }

        public async Task<List<User>> GetAllStudentByParentId(int parentId)
        {
            return await _userParentStudentRepository.GetAllStudentByParentId(parentId);    
        }

        public async Task LinkAccountChild(LinkAccount linkAccount)
        {
            var student = await _userRepository.GetUserByEmailAsync(linkAccount.Email);
            if (student == null)
            {
                throw new KeyNotFoundException("Người dùng không tồn tại.");
            }

            var existingLink = await _userParentStudentRepository.GetByParentIdAndStudentId(linkAccount.ParentId, student.UserId);
            if (existingLink != null)
            {
                if (existingLink.IsVerified)
                {
                    throw new InvalidOperationException("Tài khoản này đã được liên kết với học sinh này.");
                }

                existingLink.VerificationCode = GenerateVerificationCode();
                existingLink.ExpiryTime = DateTime.UtcNow.AddMinutes(10);
                await _userParentStudentRepository.UpdateParentStudentAsync(existingLink);
            }
            else
            {
                existingLink = new UserParentStudent
                {
                    ParentId = linkAccount.ParentId,
                    StudentId = student.UserId,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(10),
                    IsVerified = false,
                    VerificationCode = GenerateVerificationCode()
                };
                await _userParentStudentRepository.CreateParentStudentAsync(existingLink);
            }
            await _sendMailService.SendEmailLinkAccountAsync(linkAccount.Email, existingLink.VerificationCode);
        }

        public async Task<bool> VerifyLinkAccountAsync(int parentId, string studentEmail, string code)
        {
            var student = await _userRepository.GetUserByEmailAsync(studentEmail);
            if (student == null)
            {
                throw new KeyNotFoundException("Người dùng không tồn tại.");
            }

            var request = await _userParentStudentRepository.GetByParentIdAndStudentId(parentId, student.UserId);

            if (request == null || request.VerificationCode != code)
            {
                throw new KeyNotFoundException("Mã xác nhận không hợp lệ.");
            }

            if (request.ExpiryTime < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Mã xác nhận đã hết hạn.");
            }

            request.IsVerified = true;
            request.VerificationCode = "0";
            request.ExpiryTime = null;

            await _userParentStudentRepository.UpdateParentStudentAsync(request);
            return true;
        }

        public async Task<string> ResendVerificationCodeAsync(string email, int parentId)
        {
            var student = await _userRepository.GetUserByEmailAsync(email);
            if (student == null)
            {
                return "Người dùng không tồn tại.";
            }

            var parentStudent = await _userParentStudentRepository.GetByParentIdAndStudentId(parentId, student.UserId);
            if (parentStudent == null)
            {
                return "Không tìm thấy liên kết tài khoản.";
            }

            var newCode = GenerateVerificationCode();
            parentStudent.VerificationCode = newCode;
            parentStudent.ExpiryTime = DateTime.UtcNow.AddMinutes(10); 

            await _userParentStudentRepository.UpdateParentStudentAsync(parentStudent);

            await _sendMailService.SendEmailLinkAccountAsync(email, newCode);

            return "Mã xác nhận mới đã được gửi.";
        }

        public async Task DeleteParentStudentAsync(int parentId, int studentId)
        {
            await _userParentStudentRepository.DeleteParentStudentAsync(parentId, studentId);
        }


        public string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}
