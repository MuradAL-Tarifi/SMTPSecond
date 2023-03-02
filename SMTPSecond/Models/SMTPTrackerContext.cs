﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace SMTPSecond.Models
{
    public partial class SMTPTrackerContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public SMTPTrackerContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SMTPTrackerContext(DbContextOptions<SMTPTrackerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AlertBySensor> AlertBySensor { get; set; }
        public virtual DbSet<AlertTracker> AlertTracker { get; set; }
        public virtual DbSet<Smtpchecker> Smtpchecker { get; set; }
        public virtual DbSet<Smtpsetting> Smtpsetting { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration["ConnectionString:DBConnectionString"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AlertBySensor>(entity =>
            {
                entity.Property(e => e.Serial).HasMaxLength(50);
            });

            modelBuilder.Entity<AlertTracker>(entity =>
            {
                entity.Property(e => e.AlertDateTime).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(100);
            });

            modelBuilder.Entity<Smtpchecker>(entity =>
            {
                entity.ToTable("SMTPChecker");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Smtpsetting>(entity =>
            {
                entity.ToTable("SMTPSetting");

                entity.Property(e => e.MailAddress).IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.UserName).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
