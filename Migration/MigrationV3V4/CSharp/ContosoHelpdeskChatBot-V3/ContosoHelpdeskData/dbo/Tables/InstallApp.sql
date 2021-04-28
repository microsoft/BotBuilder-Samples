CREATE TABLE [dbo].[InstallApp] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [AppName]     VARCHAR (MAX) NULL,
    [MachineName] VARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.InstallApp] PRIMARY KEY CLUSTERED ([Id] ASC)
);

