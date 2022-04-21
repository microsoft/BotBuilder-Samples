// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
using Microsoft.EntityFrameworkCore;

namespace Bot.Builder.Azure.V3V4.AzureSql
{
    /// <summary>
    /// THe Microsoft.EntityFrameworkCore DbContext for <see cref="SqlBotDataEntity"/> also <see cref="SqlBotDataStore"/>. 
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    internal class SqlBotDataContext : DbContext
    {
        string _connectionString;
        public SqlBotDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SqlBotDataEntity>()
                .ToTable("SqlBotDataEntities");

            modelBuilder.Entity<SqlBotDataEntity>()
                .HasIndex(p => new { p.BotStoreType, p.ChannelId, p.UserId })
                .HasName("idxStoreChannelUser");

            modelBuilder.Entity<SqlBotDataEntity>()
                .HasIndex(p => new { p.BotStoreType, p.ChannelId, p.ConversationId })
                .HasName("idxStoreChannelConversation");

            modelBuilder.Entity<SqlBotDataEntity>()
                .HasIndex(p => new { p.BotStoreType, p.ChannelId, p.ConversationId, p.UserId })
                .HasName("idxStoreChannelConversationUser");
        }

        public virtual DbSet<SqlBotDataEntity> BotData { get; set; }
    }
}
