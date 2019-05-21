// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Data.Entity;

namespace ContosoHelpdeskChatBot.Models
{
    public partial class ContosoHelpdeskContext : DbContext
    {
        public ContosoHelpdeskContext()
            : base("name=ContosoHelpdeskContext")
        {
        }

        public virtual DbSet<AppMsi> AppMsis { get; set; }
        public virtual DbSet<InstallApp> InstallApps { get; set; }
        public virtual DbSet<LocalAdmin> LocalAdmins { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<ResetPassword> ResetPasswords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppMsi>()
                .Property(e => e.AppName)
                .IsUnicode(false);

            modelBuilder.Entity<AppMsi>()
                .Property(e => e.MsiPackage)
                .IsUnicode(false);

            modelBuilder.Entity<InstallApp>()
                .Property(e => e.AppName)
                .IsUnicode(false);

            modelBuilder.Entity<InstallApp>()
                .Property(e => e.MachineName)
                .IsUnicode(false);

            modelBuilder.Entity<ResetPassword>()
                .Property(e => e.EmailAddress)
                .IsUnicode(false);
        }
    }
}
