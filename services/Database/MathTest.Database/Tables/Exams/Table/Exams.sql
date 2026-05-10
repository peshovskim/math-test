CREATE TABLE [dbo].[Exams]
(
    [Id]               INT            NOT NULL IDENTITY (1, 1),
    [StudentUserId]    INT            NULL,
    [TeacherUserId]    INT            NULL,
    [FileName]         NVARCHAR(512)  NOT NULL,
    [ExamExternalId]   NVARCHAR(256)  NOT NULL,
    [Score]            FLOAT          NOT NULL,

    CONSTRAINT [PK_Exams] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_Exams_User_Student] FOREIGN KEY ([StudentUserId]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Exams_User_Teacher] FOREIGN KEY ([TeacherUserId]) REFERENCES [dbo].[User] ([Id])
);
GO
