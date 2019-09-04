﻿
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SqlBotDataEntities](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BotStoreType] [int] NOT NULL,
	[BotId] [nvarchar](max) NULL,
	[ChannelId] [nvarchar](200) NULL,
	[ConversationId] [nvarchar](200) NULL,
	[UserId] [nvarchar](200) NULL,
	[Data] [varbinary](max) NULL,
	[ETag] [nvarchar](max) NULL,
	[ServiceUrl] [nvarchar](max) NULL,
	[Timestamp] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_dbo.SqlBotDataEntities] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [idxStoreChannelConversation] ON [dbo].[SqlBotDataEntities]
(
	[BotStoreType] ASC,
	[ChannelId] ASC,
	[ConversationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [idxStoreChannelConversationUser] ON [dbo].[SqlBotDataEntities]
(
	[BotStoreType] ASC,
	[ChannelId] ASC,
	[ConversationId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [idxStoreChannelUser] ON [dbo].[SqlBotDataEntities]
(
	[BotStoreType] ASC,
	[ChannelId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [dbo].[SqlBotDataEntities] ADD  DEFAULT (getutcdate()) FOR [Timestamp]
GO
