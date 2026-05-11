CREATE TABLE [dbo].[User](
    [Id]                    INT                 NOT NULL IDENTITY(1, 1),
    [FirstName]             NVARCHAR(256)       NULL,
    [LastName]              NVARCHAR(256)       NULL,
    [Email]                 NVARCHAR(256)       NOT NULL,
    [PasswordHash]          VARBINARY(MAX)      NOT NULL,
    [Salt]                  VARBINARY(MAX)      NOT NULL,
    [ExternalId]            NVARCHAR(256)       NULL,

    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id])
);
GO

