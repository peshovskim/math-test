CREATE TABLE [dbo].[ExamTask]
(
    [Id]              INT            NOT NULL IDENTITY (1, 1),
    [ExamId]         INT            NOT NULL,
    [ExternalId]     NVARCHAR(256)  NOT NULL,
    [Expression]     NVARCHAR(MAX)  NOT NULL,
    [StudentAnswer]  FLOAT          NOT NULL,
    [CorrectAnswer]  FLOAT          NULL,
    [IsCorrect]      BIT            NOT NULL,

    CONSTRAINT [PK_ExamTask] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_ExamTask_Exams] FOREIGN KEY ([ExamId]) REFERENCES [dbo].[Exams] ([Id]) ON DELETE CASCADE
);
GO
