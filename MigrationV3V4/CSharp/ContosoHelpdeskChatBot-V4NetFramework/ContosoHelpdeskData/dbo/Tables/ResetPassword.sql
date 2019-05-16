CREATE TABLE [dbo].[ResetPassword] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [EmailAddress] VARCHAR (MAX)  NULL,
    [MobileNumber] BIGINT         NULL,
    [PassCode]     INT            NULL,
    [TempPassword] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.ResetPassword] PRIMARY KEY CLUSTERED ([Id] ASC)
);



