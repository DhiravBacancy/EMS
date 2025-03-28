﻿using EMS.Enums;
using EMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.Reflection.Emit;

namespace EMS.Configurations
{

    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.EmployeeId);

            builder.Property(e => e.EmployeeId)
                .ValueGeneratedOnAdd()
                .IsRequired(true);

            builder.Property(e => e.FirstName)
                .HasColumnType("VARCHAR(100)")
                .IsRequired(true);

            builder.Property(e => e.LastName)
                .HasColumnType("VARCHAR(100)")
                .IsRequired(true);

            builder.Property(e => e.Email)
                .HasColumnType("VARCHAR(255)")
                .IsRequired(true);

            builder.HasIndex(e => e.Email)
                .IsUnique(true);

            builder.Property(e => e.Password)
                .IsRequired(true);

            builder.Property(e => e.Phone)
                .HasColumnType("VARCHAR(15)")
                .IsRequired(true);

            builder.Property(e => e.DateOfBirth)
                 .IsRequired(false);

            builder.Property(e => e.Address)
                .HasColumnType("TEXT")
                .IsRequired(false);


            // Employee - Department relationship (Many-to-One)
            builder.HasOne(e => e.Department)
                .WithMany(d => d.Employees) 
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Employee - Leave relationship (One-to-Many)
            builder.HasMany(e => e.Leaves)
                .WithOne(l => l.Employee) 
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade); 

            // Employee - TimeSheet relationship (One-to-Many)
            builder.HasMany(e => e.TimeSheets)
                .WithOne(ts => ts.Employee) 
                .HasForeignKey(ts => ts.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.TechStack)
                .HasColumnType("TEXT")
                .IsRequired(false);

            builder.Property(e => e.Role)
                 .HasConversion(
                    v => v.ToString(),
                    v => (RolesEnum)Enum.Parse(typeof(RolesEnum), v)
                )
                .IsRequired();
        }
    }

}
