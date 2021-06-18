CREATE TABLE [dbo].[Log] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Date]      DATETIME       NOT NULL,
    [Level]     NVARCHAR (MAX) NULL,
    [Message]   NVARCHAR (MAX) NULL,
    [Exception] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED ([Id] ASC)
);

