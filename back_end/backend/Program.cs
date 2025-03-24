using backend.DataAccess.DAO;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using backend.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext")));
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
builder.Services.AddScoped<ICurriculumRepository, CurriculumRepository>();
builder.Services.AddScoped<RegisterService>();
builder.Services.AddScoped<PasswordResetService>();
builder.Services.AddScoped<SendMailService>();
builder.Services.AddScoped<ChapterService>();
builder.Services.AddScoped<AuthDAO>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<SubscriptionPlanService>();
builder.Services.AddScoped<SubscriptionPlanDAO>();
builder.Services.AddScoped<CurriculumDAO>();
builder.Services.AddScoped<UserDAO>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoleDAO>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<LessonService>();
builder.Services.AddScoped<PasswordEncryption>();
builder.Services.AddScoped<GradeService>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<GradeDAO>();
builder.Services.AddScoped<QuestionService>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<QuestionDAO>();
builder.Services.AddScoped<TestCategoryDAO>();
builder.Services.AddScoped<ITestCategoryRepository, TestCategoryRepository>();
builder.Services.AddScoped<TestCategoryService>();
builder.Services.AddScoped<TestService>();
builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<TestDAO>();
builder.Services.AddScoped<LevelDAO>();
builder.Services.AddScoped<ILevelRepository, LevelRepository>();
builder.Services.AddScoped<LevelService>();
builder.Services.AddScoped<TestQuestionDAO>();
builder.Services.AddScoped<ITestQuestionRepository, TestQuestionRepository>();
builder.Services.AddScoped<TestQuestionService>();
builder.Services.AddScoped<PracticeAttemptDAO>();
builder.Services.AddScoped<IPracticeRepository, PracticeRepository>();
builder.Services.AddScoped<PracticeAttemptService>();
builder.Services.AddScoped<AIQuestionDAO>();
builder.Services.AddScoped<IAIQuestionRepository, AIQuestionRepository>();
builder.Services.AddScoped<AIQuestionService>();
builder.Services.AddScoped<IStudentPerformanceRepository, StudentPerformanceRepository>();
builder.Services.AddScoped<StudentPerformanceDAO>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<StudentPerformanceService>();
builder.Services.AddScoped<StudentTestService>();
builder.Services.AddScoped<IStudentTestRepository, StudentTestRepository>();
builder.Services.AddScoped<StudentTestDAO>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<ContentManageService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ReportDAO>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ISubjectGradeRepository, SubjectGradeRepository>();
builder.Services.AddScoped<SubjectGradeDAO>();
builder.Services.AddScoped<SubjectGradeService>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<SubjectDAO>();
builder.Services.AddScoped<SubjectService>();
builder.Services.AddScoped<IUserParentStudentRepository, UserParentStudentRepository>();
builder.Services.AddScoped<UserParentStudentDAO>();
builder.Services.AddScoped<UserParentStudentService>();
builder.Services.AddHttpClient();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddScoped<StudentPerformanceDAO>();
builder.Services.AddScoped<IStudentPerformanceRepository, StudentPerformanceRepository>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000") 
            .AllowCredentials()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

var key = Encoding.ASCII.GetBytes("UltraSecureKey_ForJWTAuth!987654321@2025$");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


builder.Services.AddHttpClient();
builder.Services.AddAuthorization();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
}); ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }