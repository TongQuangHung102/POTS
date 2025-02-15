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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

var key = Encoding.ASCII.GetBytes("UltraSecureKey_ForJWTAuth!987654321@2025$");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

    googleOptions.Scope.Add("openid");
    googleOptions.Scope.Add("profile");
    googleOptions.Scope.Add("email");
});
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
