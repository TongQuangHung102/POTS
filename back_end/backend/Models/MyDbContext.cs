using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace backend.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext() { }
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var conf = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer(conf.GetConnectionString("DbContext"));
            }
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerQuestion> AnswerQuestions { get; set; }
        public DbSet<AIQuestion> AIQuestions { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<StudentPerformance> StudentPerformances { get; set; }
        public DbSet<StudentProgress> StudentProgresses { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<ContestParticipant> ContestParticipants { get; set; }
        public DbSet<CompetitionResult> CompetitionResults { get; set; }
        public DbSet<ContestQuestion> ContestQuestions { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<TestCategory> TestCategories { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Grades> Grades { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentProgress>()
                .HasKey(sp => new { sp.UserId, sp.LessonId });

            modelBuilder.Entity<StudentPerformance>()
                .HasKey(sp => new { sp.UserId, sp.LessonId });

            modelBuilder.Entity<CompetitionResult>()
                .HasKey(cr => new { cr.UserId, cr.ContestId });

            modelBuilder.Entity<Prerequisite>()
                .HasKey(p => new { p.ChapterId, p.TestId });

            modelBuilder.Entity<UserParentStudent>()
                .HasOne(ups => ups.Student)
                .WithMany()
                .HasForeignKey(ups => ups.StudentId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<UserParentStudent>()
                .HasOne(ups => ups.Parent)
                .WithMany()
                .HasForeignKey(ups => ups.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuizAttempt>()
                .HasOne(qa => qa.User)
                .WithMany(u => u.QuizAttempts)
                .HasForeignKey(qa => qa.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentPerformance>()
                .HasOne(sp => sp.User)
                .WithMany(u => u.StudentPerformances)
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentProgress>()
                .HasOne(sp => sp.User)
                .WithMany(u => u.StudentProgresses)
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.QuizAttempt) 
                .WithMany() 
                .HasForeignKey(sa => sa.AttemptId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chapter>()
                .HasOne(c => c.Grade)
                .WithMany(g => g.Chapters)
                .HasForeignKey(c => c.GradeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Test>()
               .HasOne(t => t.Grade)
               .WithMany(g => g.Tests)
               .HasForeignKey(t => t.GradeId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Grade)
                .WithMany(g => g.Users) 
                .HasForeignKey(u => u.GradeId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
