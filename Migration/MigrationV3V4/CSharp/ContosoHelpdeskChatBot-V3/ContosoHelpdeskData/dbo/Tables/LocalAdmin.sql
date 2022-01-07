CREATE TABLE [dbo].[LocalAdmin] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [MachineName]   NVARCHAR (MAX) NULL,
    [AdminDuration] INT            NULL,
    CONSTRAINT [PK_dbo.LocalAdmin] PRIMARY KEY CLUSTERED ([Id] ASC)
);

