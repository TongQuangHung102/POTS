IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Prerequisite] DROP CONSTRAINT [FK_Prerequisite_Test_TestId];
GO

ALTER TABLE [StudentTest] DROP CONSTRAINT [FK_StudentTest_Test_TestId];
GO

ALTER TABLE [TestQuestion] DROP CONSTRAINT [FK_TestQuestion_Test_TestId];
GO

ALTER TABLE [Test] DROP CONSTRAINT [PK_Test];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Test]') AND [c].[name] = N'Duration');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Test] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Test] DROP COLUMN [Duration];
GO

EXEC sp_rename N'[Test]', N'Tests';
GO

ALTER TABLE [Tests] ADD [DurationInMinutes] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Tests] ADD CONSTRAINT [PK_Tests] PRIMARY KEY ([TestId]);
GO

ALTER TABLE [Prerequisite] ADD CONSTRAINT [FK_Prerequisite_Tests_TestId] FOREIGN KEY ([TestId]) REFERENCES [Tests] ([TestId]) ON DELETE CASCADE;
GO

ALTER TABLE [StudentTest] ADD CONSTRAINT [FK_StudentTest_Tests_TestId] FOREIGN KEY ([TestId]) REFERENCES [Tests] ([TestId]) ON DELETE CASCADE;
GO

ALTER TABLE [TestQuestion] ADD CONSTRAINT [FK_TestQuestion_Tests_TestId] FOREIGN KEY ([TestId]) REFERENCES [Tests] ([TestId]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250227071919_AddDurationInMinutesToTest', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Tests] ADD [GradeId] int NOT NULL DEFAULT 1;
GO

CREATE INDEX [IX_Tests_GradeId] ON [Tests] ([GradeId]);
GO

ALTER TABLE [Tests] ADD CONSTRAINT [FK_Tests_Grades_GradeId] FOREIGN KEY ([GradeId]) REFERENCES [Grades] ([GradeId]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250227133004_AddGradeForeignKeyToTests', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Grades] ADD [UserId] int NULL;
GO

CREATE INDEX [IX_Grades_UserId] ON [Grades] ([UserId]);
GO

ALTER TABLE [Grades] ADD CONSTRAINT [FK_Grades_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250303032324_AddUserIdGrade', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [TestQuestion] DROP CONSTRAINT [FK_TestQuestion_Questions_QuestionId];
GO

ALTER TABLE [TestQuestion] DROP CONSTRAINT [FK_TestQuestion_Tests_TestId];
GO

ALTER TABLE [TestSubmission] DROP CONSTRAINT [FK_TestSubmission_Questions_QuestionId];
GO

ALTER TABLE [TestSubmission] DROP CONSTRAINT [FK_TestSubmission_StudentTest_StudentTestId];
GO

ALTER TABLE [TestSubmission] DROP CONSTRAINT [PK_TestSubmission];
GO

ALTER TABLE [TestQuestion] DROP CONSTRAINT [PK_TestQuestion];
GO

EXEC sp_rename N'[TestSubmission]', N'TestSubmissions';
GO

EXEC sp_rename N'[TestQuestion]', N'TestQuestions';
GO

EXEC sp_rename N'[TestSubmissions].[IX_TestSubmission_StudentTestId]', N'IX_TestSubmissions_StudentTestId', N'INDEX';
GO

EXEC sp_rename N'[TestSubmissions].[IX_TestSubmission_QuestionId]', N'IX_TestSubmissions_QuestionId', N'INDEX';
GO

EXEC sp_rename N'[TestQuestions].[IX_TestQuestion_TestId]', N'IX_TestQuestions_TestId', N'INDEX';
GO

EXEC sp_rename N'[TestQuestions].[IX_TestQuestion_QuestionId]', N'IX_TestQuestions_QuestionId', N'INDEX';
GO

ALTER TABLE [TestSubmissions] ADD CONSTRAINT [PK_TestSubmissions] PRIMARY KEY ([SubmissionId]);
GO

ALTER TABLE [TestQuestions] ADD CONSTRAINT [PK_TestQuestions] PRIMARY KEY ([TestQuestionId]);
GO

ALTER TABLE [TestQuestions] ADD CONSTRAINT [FK_TestQuestions_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([QuestionId]) ON DELETE CASCADE;
GO

ALTER TABLE [TestQuestions] ADD CONSTRAINT [FK_TestQuestions_Tests_TestId] FOREIGN KEY ([TestId]) REFERENCES [Tests] ([TestId]) ON DELETE CASCADE;
GO

ALTER TABLE [TestSubmissions] ADD CONSTRAINT [FK_TestSubmissions_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([QuestionId]) ON DELETE CASCADE;
GO

ALTER TABLE [TestSubmissions] ADD CONSTRAINT [FK_TestSubmissions_StudentTest_StudentTestId] FOREIGN KEY ([StudentTestId]) REFERENCES [StudentTest] ([StudentTestId]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250304021256_FixTestQuestions', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [StudentAnswers] DROP CONSTRAINT [FK_StudentAnswers_QuizAttempts_AttemptId];
GO

ALTER TABLE [StudentAnswers] DROP CONSTRAINT [FK_StudentAnswers_QuizAttempts_QuizAttemptAttemptId];
GO

DROP TABLE [QuizAttempts];
GO

EXEC sp_rename N'[StudentAnswers].[QuizAttemptAttemptId]', N'PracticeAttemptPracticeId', N'COLUMN';
GO

EXEC sp_rename N'[StudentAnswers].[IX_StudentAnswers_QuizAttemptAttemptId]', N'IX_StudentAnswers_PracticeAttemptPracticeId', N'INDEX';
GO

CREATE TABLE [PracticeAttempts] (
    [PracticeId] int NOT NULL IDENTITY,
    [CorrectAnswers] int NOT NULL,
    [Level] int NOT NULL,
    [Time] time NOT NULL,
    [UserId] int NOT NULL,
    [LessonId] int NOT NULL,
    [LevelId] int NULL,
    CONSTRAINT [PK_PracticeAttempts] PRIMARY KEY ([PracticeId]),
    CONSTRAINT [FK_PracticeAttempts_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([LessonId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PracticeAttempts_Levels_LevelId] FOREIGN KEY ([LevelId]) REFERENCES [Levels] ([LevelId]),
    CONSTRAINT [FK_PracticeAttempts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_PracticeAttempts_LessonId] ON [PracticeAttempts] ([LessonId]);
GO

CREATE INDEX [IX_PracticeAttempts_LevelId] ON [PracticeAttempts] ([LevelId]);
GO

CREATE INDEX [IX_PracticeAttempts_UserId] ON [PracticeAttempts] ([UserId]);
GO

ALTER TABLE [StudentAnswers] ADD CONSTRAINT [FK_StudentAnswers_PracticeAttempts_AttemptId] FOREIGN KEY ([AttemptId]) REFERENCES [PracticeAttempts] ([PracticeId]) ON DELETE NO ACTION;
GO

ALTER TABLE [StudentAnswers] ADD CONSTRAINT [FK_StudentAnswers_PracticeAttempts_PracticeAttemptPracticeId] FOREIGN KEY ([PracticeAttemptPracticeId]) REFERENCES [PracticeAttempts] ([PracticeId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250306025713_RenameQuizAttemptToPracticeAttempt', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [PracticeAttempts] DROP CONSTRAINT [FK_PracticeAttempts_Levels_LevelId];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PracticeAttempts]') AND [c].[name] = N'Level');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [PracticeAttempts] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [PracticeAttempts] DROP COLUMN [Level];
GO

DROP INDEX [IX_PracticeAttempts_LevelId] ON [PracticeAttempts];
DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PracticeAttempts]') AND [c].[name] = N'LevelId');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [PracticeAttempts] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [PracticeAttempts] ALTER COLUMN [LevelId] int NOT NULL;
ALTER TABLE [PracticeAttempts] ADD DEFAULT 0 FOR [LevelId];
CREATE INDEX [IX_PracticeAttempts_LevelId] ON [PracticeAttempts] ([LevelId]);
GO

ALTER TABLE [PracticeAttempts] ADD CONSTRAINT [FK_PracticeAttempts_Levels_LevelId] FOREIGN KEY ([LevelId]) REFERENCES [Levels] ([LevelId]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250306061625_RemoveLevelIdFromPracticeAttempt', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [PracticeAttempts] ADD [SampleQuestion] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250307082753_UpdatePracticeAttempt', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Levels] ADD [LevelNumber] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250310022339_AddNewLevelNumber', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [PracticeAttempts] ADD [CreateAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250310080335_AddNewCreateAtPractice', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StudentPerformances]') AND [c].[name] = N'avg_Time_Per_Question');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [StudentPerformances] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [StudentPerformances] DROP COLUMN [avg_Time_Per_Question];
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PracticeAttempts]') AND [c].[name] = N'Time');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [PracticeAttempts] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [PracticeAttempts] DROP COLUMN [Time];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250310122801_EditTimeSpan2', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [StudentPerformances] ADD [avg_Time] float NULL;
GO

ALTER TABLE [PracticeAttempts] ADD [TimePractice] float NOT NULL DEFAULT 0.0E0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250310123045_AddTimeDouble', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [StudentTest] DROP CONSTRAINT [FK_StudentTest_Tests_TestId];
GO

ALTER TABLE [StudentTest] DROP CONSTRAINT [FK_StudentTest_Users_UserId];
GO

ALTER TABLE [TestSubmissions] DROP CONSTRAINT [FK_TestSubmissions_StudentTest_StudentTestId];
GO

ALTER TABLE [StudentTest] DROP CONSTRAINT [PK_StudentTest];
GO

EXEC sp_rename N'[StudentTest]', N'StudentTests';
GO

EXEC sp_rename N'[StudentTests].[IX_StudentTest_UserId]', N'IX_StudentTests_UserId', N'INDEX';
GO

EXEC sp_rename N'[StudentTests].[IX_StudentTest_TestId]', N'IX_StudentTests_TestId', N'INDEX';
GO

ALTER TABLE [StudentTests] ADD CONSTRAINT [PK_StudentTests] PRIMARY KEY ([StudentTestId]);
GO

ALTER TABLE [StudentTests] ADD CONSTRAINT [FK_StudentTests_Tests_TestId] FOREIGN KEY ([TestId]) REFERENCES [Tests] ([TestId]) ON DELETE CASCADE;
GO

ALTER TABLE [StudentTests] ADD CONSTRAINT [FK_StudentTests_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE;
GO

ALTER TABLE [TestSubmissions] ADD CONSTRAINT [FK_TestSubmissions_StudentTests_StudentTestId] FOREIGN KEY ([StudentTestId]) REFERENCES [StudentTests] ([StudentTestId]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250311073706_UpdateStudentTest', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250313023143_AddAnswerQuestionsToAIQuestion', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Reports] (
    [ReportId] int NOT NULL IDENTITY,
    [QuestionId] int NOT NULL,
    [UserId] int NOT NULL,
    [Reason] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Reports] PRIMARY KEY ([ReportId]),
    CONSTRAINT [FK_Reports_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [Questions] ([QuestionId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reports_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Reports_QuestionId] ON [Reports] ([QuestionId]);
GO

CREATE INDEX [IX_Reports_UserId] ON [Reports] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250318125447_AddReportTable', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Subjects] (
    [SubjectId] int NOT NULL IDENTITY,
    [SubjectName] nvarchar(max) NOT NULL,
    [IsVisible] bit NOT NULL,
    CONSTRAINT [PK_Subjects] PRIMARY KEY ([SubjectId])
);
GO

CREATE TABLE [SubjectGrades] (
    [SubjectId] int NOT NULL,
    [GradeId] int NOT NULL,
    CONSTRAINT [PK_SubjectGrades] PRIMARY KEY ([SubjectId], [GradeId]),
    CONSTRAINT [FK_SubjectGrades_Grades_GradeId] FOREIGN KEY ([GradeId]) REFERENCES [Grades] ([GradeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SubjectGrades_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([SubjectId]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_SubjectGrades_GradeId] ON [SubjectGrades] ([GradeId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250319035035_AddTableSubject', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Chapters] DROP CONSTRAINT [FK_Chapters_Grades_GradeId];
GO

ALTER TABLE [SubjectGrades] DROP CONSTRAINT [PK_SubjectGrades];
GO

DROP INDEX [IX_Chapters_GradeId] ON [Chapters];
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Chapters]') AND [c].[name] = N'GradeId');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Chapters] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [Chapters] DROP COLUMN [GradeId];
GO

ALTER TABLE [SubjectGrades] ADD [Id] int NOT NULL IDENTITY;
GO

ALTER TABLE [SubjectGrades] ADD CONSTRAINT [PK_SubjectGrades] PRIMARY KEY ([Id]);
GO

CREATE INDEX [IX_SubjectGrades_SubjectId] ON [SubjectGrades] ([SubjectId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250319045002_edidTableChapter', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Chapters] ADD [SubjectGradeId] int NOT NULL DEFAULT 1;
GO

CREATE INDEX [IX_Chapters_SubjectGradeId] ON [Chapters] ([SubjectGradeId]);
GO

ALTER TABLE [Chapters] ADD CONSTRAINT [FK_Chapters_SubjectGrades_SubjectGradeId] FOREIGN KEY ([SubjectGradeId]) REFERENCES [SubjectGrades] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250319051256_EditTableChapter', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Tests] DROP CONSTRAINT [FK_Tests_Grades_GradeId];
GO

EXEC sp_rename N'[Tests].[GradeId]', N'SubjectGradeId', N'COLUMN';
GO

EXEC sp_rename N'[Tests].[IX_Tests_GradeId]', N'IX_Tests_SubjectGradeId', N'INDEX';
GO

ALTER TABLE [Tests] ADD CONSTRAINT [FK_Tests_SubjectGrades_SubjectGradeId] FOREIGN KEY ([SubjectGradeId]) REFERENCES [SubjectGrades] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250319093903_EditTableTest', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [UserParentStudent] DROP CONSTRAINT [FK_UserParentStudent_Users_ParentId];
GO

ALTER TABLE [UserParentStudent] DROP CONSTRAINT [FK_UserParentStudent_Users_StudentId];
GO

ALTER TABLE [UserParentStudent] DROP CONSTRAINT [PK_UserParentStudent];
GO

EXEC sp_rename N'[UserParentStudent]', N'UserParentStudents';
GO

EXEC sp_rename N'[UserParentStudents].[IX_UserParentStudent_StudentId]', N'IX_UserParentStudents_StudentId', N'INDEX';
GO

EXEC sp_rename N'[UserParentStudents].[IX_UserParentStudent_ParentId]', N'IX_UserParentStudents_ParentId', N'INDEX';
GO

ALTER TABLE [UserParentStudents] ADD CONSTRAINT [PK_UserParentStudents] PRIMARY KEY ([UserParentStudentId]);
GO

ALTER TABLE [UserParentStudents] ADD CONSTRAINT [FK_UserParentStudents_Users_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION;
GO

ALTER TABLE [UserParentStudents] ADD CONSTRAINT [FK_UserParentStudents_Users_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250322082839_AddTableUserParentStudent', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [UserParentStudents] ADD [ExpiryTime] datetime2 NULL;
GO

ALTER TABLE [UserParentStudents] ADD [IsVerified] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [UserParentStudents] ADD [VerificationCode] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250324063643_AddTokenToParentAndChild', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Reports]') AND [c].[name] = N'Reason');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Reports] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [Reports] ALTER COLUMN [Reason] int NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250324140227_EditTableReport', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Reports] ADD [ReportCount] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250324143524_EditTableReport2', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [IsRead] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ReadAt] datetime2 NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250327081239_AddTableNotification', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [StudentAnswers] DROP CONSTRAINT [FK_StudentAnswers_Questions_QuestionId];
GO

ALTER TABLE [StudentAnswers] ADD [QuestionId1] int NULL;
GO

CREATE TABLE [PracticeQuestions] (
    [QuestionId] int NOT NULL IDENTITY,
    [QuestionText] nvarchar(max) NOT NULL,
    [CreateAt] datetime2 NOT NULL,
    [LevelId] int NOT NULL,
    [CorrectAnswer] int NOT NULL,
    [LessonId] int NOT NULL,
    CONSTRAINT [PK_PracticeQuestions] PRIMARY KEY ([QuestionId]),
    CONSTRAINT [FK_PracticeQuestions_Lessons_LessonId] FOREIGN KEY ([LessonId]) REFERENCES [Lessons] ([LessonId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PracticeQuestions_Levels_LevelId] FOREIGN KEY ([LevelId]) REFERENCES [Levels] ([LevelId]) ON DELETE CASCADE
);
GO

CREATE TABLE [AnswerPracticeQuestions] (
    [AnswerQuestionId] int NOT NULL IDENTITY,
    [AnswerText] nvarchar(max) NOT NULL,
    [Number] int NOT NULL,
    [QuestionId] int NOT NULL,
    CONSTRAINT [PK_AnswerPracticeQuestions] PRIMARY KEY ([AnswerQuestionId]),
    CONSTRAINT [FK_AnswerPracticeQuestions_PracticeQuestions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [PracticeQuestions] ([QuestionId]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_StudentAnswers_QuestionId1] ON [StudentAnswers] ([QuestionId1]);
GO

CREATE INDEX [IX_AnswerPracticeQuestions_QuestionId] ON [AnswerPracticeQuestions] ([QuestionId]);
GO

CREATE INDEX [IX_PracticeQuestions_LessonId] ON [PracticeQuestions] ([LessonId]);
GO

CREATE INDEX [IX_PracticeQuestions_LevelId] ON [PracticeQuestions] ([LevelId]);
GO

ALTER TABLE [StudentAnswers] ADD CONSTRAINT [FK_StudentAnswers_PracticeQuestions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [PracticeQuestions] ([QuestionId]) ON DELETE CASCADE;
GO

ALTER TABLE [StudentAnswers] ADD CONSTRAINT [FK_StudentAnswers_Questions_QuestionId1] FOREIGN KEY ([QuestionId1]) REFERENCES [Questions] ([QuestionId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250329025813_AddTablePracticeQuestion', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [StudentAnswers] DROP CONSTRAINT [FK_StudentAnswers_PracticeAttempts_AttemptId];
GO

ALTER TABLE [StudentAnswers] DROP CONSTRAINT [FK_StudentAnswers_PracticeAttempts_PracticeAttemptPracticeId];
GO

DROP INDEX [IX_StudentAnswers_PracticeAttemptPracticeId] ON [StudentAnswers];
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StudentAnswers]') AND [c].[name] = N'PracticeAttemptPracticeId');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [StudentAnswers] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [StudentAnswers] DROP COLUMN [PracticeAttemptPracticeId];
GO

EXEC sp_rename N'[StudentAnswers].[AttemptId]', N'PracticeId', N'COLUMN';
GO

EXEC sp_rename N'[StudentAnswers].[IX_StudentAnswers_AttemptId]', N'IX_StudentAnswers_PracticeId', N'INDEX';
GO

ALTER TABLE [StudentAnswers] ADD CONSTRAINT [FK_StudentAnswers_PracticeAttempts_PracticeId] FOREIGN KEY ([PracticeId]) REFERENCES [PracticeAttempts] ([PracticeId]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250329034335_editTableStudentAnswers', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [PracticeQuestions] DROP CONSTRAINT [FK_PracticeQuestions_Levels_LevelId];
GO

DROP INDEX [IX_PracticeQuestions_LevelId] ON [PracticeQuestions];
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PracticeQuestions]') AND [c].[name] = N'LevelId');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [PracticeQuestions] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [PracticeQuestions] DROP COLUMN [LevelId];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250329060453_editTablePracticeQuestion', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250329081441_FixPracticeAttemptKey', N'6.0.29');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250329083016_FixStudentAnswerRelationship', N'6.0.29');
GO

COMMIT;
GO

