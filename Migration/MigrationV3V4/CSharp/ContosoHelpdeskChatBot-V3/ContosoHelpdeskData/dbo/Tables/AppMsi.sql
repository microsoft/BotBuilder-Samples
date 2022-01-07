CREATE TABLE [dbo].[AppMsi] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [AppName]    VARCHAR (MAX) NULL,
    [MsiPackage] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.AppMsi] PRIMARY KEY CLUSTERED ([Id] ASC)
);

