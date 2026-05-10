IF NOT EXISTS (SELECT 1 FROM [dbo].[Role] WHERE [Name] = N'Student')
BEGIN
    INSERT INTO [dbo].[Role] ([Name]) VALUES (N'Student');
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[Role] WHERE [Name] = N'Teacher')
BEGIN
    INSERT INTO [dbo].[Role] ([Name]) VALUES (N'Teacher');
END
GO
