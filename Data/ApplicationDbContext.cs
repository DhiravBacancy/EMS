using EMS.Models;
using EMS.Enums;
using Microsoft.EntityFrameworkCore;

namespace EMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<TimeSheet> TimeSheets { get; set; }
        public DbSet<Leave> Leaves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);

            // Seed Departments
            modelBuilder.Entity<Department>().HasData(SeedingData.GetDepartments());

            // Seed Admins
            modelBuilder.Entity<Admin>().HasData(SeedingData.GetAdmins());

            // Seed Employees
            modelBuilder.Entity<Employee>().HasData(SeedingData.GetEmployees());

            modelBuilder.Entity<TimeSheet>().HasData(SeedingData.GetTimeSheets());

            modelBuilder.Entity<Leave>().HasData(SeedingData.GetLeaves());
        }

        public static class SeedingData
        {
            public static List<Department> GetDepartments() => new()
            {
                new Department { DepartmentId = 1, DepartmentName = "HR", CreatedAt = new DateTime(2024, 03, 20) },
                new Department { DepartmentId = 2, DepartmentName = "IT", CreatedAt = new DateTime(2024, 03, 20)},
                new Department { DepartmentId = 3, DepartmentName = "Finance", CreatedAt = new DateTime(2024, 03, 20) },
                new Department { DepartmentId = 4, DepartmentName = "Marketing", CreatedAt = new DateTime(2024, 03, 20) }
            };

            public static List<Admin> GetAdmins() => new()
            {
                new Admin { AdminId = 1, FirstName = "John", LastName = "Doe", Email = "Admin1@gmail.com", Passsword = "admin123", Phone = "1234567890", Role = RolesEnum.Admin, CreatedAt =  new DateTime(2024, 03, 20) },
                new Admin { AdminId = 2, FirstName = "Sam", LastName = "Wilson", Email = "Admin2@gmail.com", Passsword = "admin456", Phone = "9876543211", Role = RolesEnum.Admin, CreatedAt = new DateTime(2024, 03, 21) }
            };

            public static List<Employee> GetEmployees() => new()
            {
                new Employee { EmployeeId = 1, FirstName = "Alice", LastName = "Smith", Email = "alice@gmail.com", Password = "password123", Phone = "9876543210", Address = "123 Gota, Ahmedabad", DepartmentId = 2, Role = RolesEnum.Employee,  CreatedAt =  new DateTime(2024, 03, 20) },
                new Employee { EmployeeId = 2, FirstName = "Bob", LastName = "Brown", Email = "bob@gmail.com", Password = "password123", Phone = "8765432109", Address = "456 Vesu, Surat", DepartmentId = 1, Role = RolesEnum.Employee,  CreatedAt =  new DateTime(2024, 03, 20) },
                new Employee { EmployeeId = 3, FirstName = "Dhirav", LastName = "Agrawal", Email = "Dhirav@gmail.com", Password = "password123", Phone = "9328242713", Address = "14 Thaltej, Ahmedabad", DepartmentId = 2, Role = RolesEnum.Employee,  CreatedAt =  new DateTime(2024, 03, 20) },
                new Employee { EmployeeId = 4, FirstName = "Pankaj", LastName = "Kumar", Email = "Pankaj@gmail.com", Password = "password123", Phone = "4789237943", Address = "01 OP road, Baroda", DepartmentId = 3, Role = RolesEnum.Employee,  CreatedAt =  new DateTime(2024, 03, 20) },
                new Employee { EmployeeId = 5, FirstName = "Raj", LastName = "Verma", Email = "raj@gmail.com", Password = "password123", Phone = "9988776655", Address = "789 Maninagar, Ahmedabad", DepartmentId = 4, Role = RolesEnum.Employee,  CreatedAt = new DateTime(2024, 03, 21) },
                new Employee { EmployeeId = 6, FirstName = "Meera", LastName = "Shah", Email = "meera@gmail.com", Password = "password123", Phone = "9876501234", Address = "500 Ellis Bridge, Ahmedabad", DepartmentId = 3, Role = RolesEnum.Employee,  CreatedAt = new DateTime(2024, 03, 22) },
                new Employee { EmployeeId = 7, FirstName = "Kunal", LastName = "Patel", Email = "kunal@gmail.com", Password = "password123", Phone = "9345678901", Address = "22 SG Highway, Ahmedabad", DepartmentId = 1, Role = RolesEnum.Employee,  CreatedAt = new DateTime(2024, 03, 22) }
            };

            public static List<TimeSheet> GetTimeSheets() => new()
            {
                // Employee 1 (Alice)
                new TimeSheet { TimesheetId = 1, EmployeeId = 1, Date = new DateTime(2024, 3, 1), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Worked on project X", CreatedAt = new DateTime(2024, 3, 1) },
                new TimeSheet { TimesheetId = 2, EmployeeId = 1, Date = new DateTime(2024, 3, 2), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Worked on project X", CreatedAt = new DateTime(2024, 3, 2) },
                new TimeSheet { TimesheetId = 3, EmployeeId = 1, Date = new DateTime(2024, 3, 3), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Client meeting", CreatedAt = new DateTime(2024, 3, 3) },
                new TimeSheet { TimesheetId = 4, EmployeeId = 1, Date = new DateTime(2024, 3, 4), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Project planning", CreatedAt = new DateTime(2024, 3, 4) },
                new TimeSheet { TimesheetId = 5, EmployeeId = 1, Date = new DateTime(2024, 3, 5), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Code review", CreatedAt = new DateTime(2024, 3, 5) },

                // Employee 2 (Bob)
                new TimeSheet { TimesheetId = 6, EmployeeId = 2, Date = new DateTime(2024, 3, 1), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0), Description = "Client meeting and development", CreatedAt = new DateTime(2024, 3, 1) },
                new TimeSheet { TimesheetId = 7, EmployeeId = 2, Date = new DateTime(2024, 3, 2), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0), Description = "System design", CreatedAt = new DateTime(2024, 3, 2) },
                new TimeSheet { TimesheetId = 8, EmployeeId = 2, Date = new DateTime(2024, 3, 3), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0), Description = "Code implementation", CreatedAt = new DateTime(2024, 3, 3) },
                new TimeSheet { TimesheetId = 9, EmployeeId = 2, Date = new DateTime(2024, 3, 4), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0), Description = "Team meeting and discussion", CreatedAt = new DateTime(2024, 3, 4) },
                new TimeSheet { TimesheetId = 10, EmployeeId = 2, Date = new DateTime(2024, 3, 5), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0), Description = "Feature testing", CreatedAt = new DateTime(2024, 3, 5) },

                // Employee 3 (Dhirav)
                new TimeSheet { TimesheetId = 11, EmployeeId = 3, Date = new DateTime(2024, 3, 1), StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(16, 30, 0), Description = "System maintenance", CreatedAt = new DateTime(2024, 3, 1) },
                new TimeSheet { TimesheetId = 12, EmployeeId = 3, Date = new DateTime(2024, 3, 2), StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(16, 30, 0), Description = "Database optimization", CreatedAt = new DateTime(2024, 3, 2) },
                new TimeSheet { TimesheetId = 13, EmployeeId = 3, Date = new DateTime(2024, 3, 3), StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(16, 30, 0), Description = "User feedback analysis", CreatedAt = new DateTime(2024, 3, 3) },
                new TimeSheet { TimesheetId = 14, EmployeeId = 3, Date = new DateTime(2024, 3, 4), StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(16, 30, 0), Description = "API integration", CreatedAt = new DateTime(2024, 3, 4) },
                new TimeSheet { TimesheetId = 15, EmployeeId = 3, Date = new DateTime(2024, 3, 5), StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(16, 30, 0), Description = "Bug fixing", CreatedAt = new DateTime(2024, 3, 5) },

                // Employee 4 (Pankaj)
                new TimeSheet { TimesheetId = 16, EmployeeId = 4, Date = new DateTime(2024, 3, 1), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Project setup", CreatedAt = new DateTime(2024, 3, 1) },
                new TimeSheet { TimesheetId = 17, EmployeeId = 4, Date = new DateTime(2024, 3, 2), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Project setup", CreatedAt = new DateTime(2024, 3, 2) },
                new TimeSheet { TimesheetId = 18, EmployeeId = 4, Date = new DateTime(2024, 3, 3), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Team coordination", CreatedAt = new DateTime(2024, 3, 3) },
                new TimeSheet { TimesheetId = 19, EmployeeId = 4, Date = new DateTime(2024, 3, 4), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Client meeting", CreatedAt = new DateTime(2024, 3, 4) },
                new TimeSheet { TimesheetId = 20, EmployeeId = 4, Date = new DateTime(2024, 3, 5), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Business strategy discussion", CreatedAt = new DateTime(2024, 3, 5) },

                // Employee 5 (Raj)
                new TimeSheet { TimesheetId = 21, EmployeeId = 5, Date = new DateTime(2024, 3, 1), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Product research", CreatedAt = new DateTime(2024, 3, 1) },
                new TimeSheet { TimesheetId = 22, EmployeeId = 5, Date = new DateTime(2024, 3, 2), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Feature design", CreatedAt = new DateTime(2024, 3, 2) },
                new TimeSheet { TimesheetId = 23, EmployeeId = 5, Date = new DateTime(2024, 3, 3), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Feature implementation", CreatedAt = new DateTime(2024, 3, 3) },
                new TimeSheet { TimesheetId = 24, EmployeeId = 5, Date = new DateTime(2024, 3, 4), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "System testing", CreatedAt = new DateTime(2024, 3, 4) },
                new TimeSheet { TimesheetId = 25, EmployeeId = 5, Date = new DateTime(2024, 3, 5), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "User feedback review", CreatedAt = new DateTime(2024, 3, 5) },

                // Employee 6 (Meera)
                new TimeSheet { TimesheetId = 26, EmployeeId = 6, Date = new DateTime(2024, 3, 1), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Database cleanup", CreatedAt = new DateTime(2024, 3, 1) },
                new TimeSheet { TimesheetId = 27, EmployeeId = 6, Date = new DateTime(2024, 3, 2), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Report generation", CreatedAt = new DateTime(2024, 3, 2) },
                new TimeSheet { TimesheetId = 28, EmployeeId = 6, Date = new DateTime(2024, 3, 3), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Client follow-up", CreatedAt = new DateTime(2024, 3, 3) },
                new TimeSheet { TimesheetId = 29, EmployeeId = 6, Date = new DateTime(2024, 3, 4), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "System monitoring", CreatedAt = new DateTime(2024, 3, 4) },
                new TimeSheet { TimesheetId = 30, EmployeeId = 6, Date = new DateTime(2024, 3, 5), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), Description = "Security patch implementation", CreatedAt = new DateTime(2024, 3, 5) },

                // Employee 7 (Kunal)
                new TimeSheet { TimesheetId = 31, EmployeeId = 7, Date = new DateTime(2024, 3, 1), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0), Description = "Product testing", CreatedAt = new DateTime(2024, 3, 1) },
                new TimeSheet { TimesheetId = 32, EmployeeId = 7, Date = new DateTime(2024, 3, 2), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0), Description = "Team coordination", CreatedAt = new DateTime(2024, 3, 2) },
                new TimeSheet { TimesheetId = 33, EmployeeId = 7, Date = new DateTime(2024, 3, 3), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0), Description = "Client requirements gathering", CreatedAt = new DateTime(2024, 3, 3) },
                new TimeSheet { TimesheetId = 34, EmployeeId = 7, Date = new DateTime(2024, 3, 4), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0), Description = "Feature implementation", CreatedAt = new DateTime(2024, 3, 4) },
                new TimeSheet { TimesheetId = 35, EmployeeId = 7, Date = new DateTime(2024, 3, 5), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(18, 0, 0), Description = "Bug fixes and testing", CreatedAt = new DateTime(2024, 3, 5) },

            };

            public static List<Leave> GetLeaves() => new()
            {
                // Leaves for Employee 1 (Alice)
                new Leave { LeaveId = 1, EmployeeId = 1, StartDate = new DateTime(2024, 3, 5), EndDate = new DateTime(2024, 3, 7), TotalDays = 3, LeaveType = LeaveTypeEnum.SickLeave, Reason = "Flu", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 3) },
                new Leave { LeaveId = 2, EmployeeId = 1, StartDate = new DateTime(2024, 3, 15), EndDate = new DateTime(2024, 3, 17), TotalDays = 3, LeaveType = LeaveTypeEnum.CasualLeave, Reason = "Family function", Status = StatusEnum.Approved, AppliedAt = new DateTime(2024, 3, 10) },
                new Leave { LeaveId = 3, EmployeeId = 1, StartDate = new DateTime(2024, 3, 25), EndDate = new DateTime(2024, 3, 27), TotalDays = 3, LeaveType = LeaveTypeEnum.UnpaidLeave, Reason = "Personal reasons", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 20) },

                // Leaves for Employee 2 (Bob)
                new Leave { LeaveId = 4, EmployeeId = 2, StartDate = new DateTime(2024, 3, 10), EndDate = new DateTime(2024, 3, 12), TotalDays = 3, LeaveType = LeaveTypeEnum.SickLeave, Reason = "Flu", Status = StatusEnum.Approved, AppliedAt = new DateTime(2024, 3, 8) },
                new Leave { LeaveId = 5, EmployeeId = 2, StartDate = new DateTime(2024, 3, 20), EndDate = new DateTime(2024, 3, 22), TotalDays = 3, LeaveType = LeaveTypeEnum.Vacation, Reason = "Vacation", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 18) },
                new Leave { LeaveId = 6, EmployeeId = 2, StartDate = new DateTime(2024, 3, 30), EndDate = new DateTime(2024, 4, 1), TotalDays = 3, LeaveType = LeaveTypeEnum.UnpaidLeave, Reason = "Personal matters", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 25) },

                // Leaves for Employee 3 (Dhirav)
                new Leave { LeaveId = 7, EmployeeId = 3, StartDate = new DateTime(2024, 3, 8), EndDate = new DateTime(2024, 3, 10), TotalDays = 3, LeaveType = LeaveTypeEnum.SickLeave, Reason = "Health issues", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 5) },
                new Leave { LeaveId = 8, EmployeeId = 3, StartDate = new DateTime(2024, 3, 18), EndDate = new DateTime(2024, 3, 20), TotalDays = 3, LeaveType = LeaveTypeEnum.CasualLeave, Reason = "Family visit", Status = StatusEnum.Approved, AppliedAt = new DateTime(2024, 3, 15) },
                new Leave { LeaveId = 9, EmployeeId = 3, StartDate = new DateTime(2024, 3, 25), EndDate = new DateTime(2024, 3, 27), TotalDays = 3, LeaveType = LeaveTypeEnum.UnpaidLeave, Reason = "Personal development", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 22) },

                // Leaves for Employee 4 (Pankaj)
                new Leave { LeaveId = 10, EmployeeId = 4, StartDate = new DateTime(2024, 3, 6), EndDate = new DateTime(2024, 3, 8), TotalDays = 3, LeaveType = LeaveTypeEnum.SickLeave, Reason = "Sore throat", Status = StatusEnum.Approved, AppliedAt = new DateTime(2024, 3, 4) },
                new Leave { LeaveId = 11, EmployeeId = 4, StartDate = new DateTime(2024, 3, 12), EndDate = new DateTime(2024, 3, 14), TotalDays = 3, LeaveType = LeaveTypeEnum.UnpaidLeave, Reason = "Wedding", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 9) },
                new Leave { LeaveId = 12, EmployeeId = 4, StartDate = new DateTime(2024, 3, 20), EndDate = new DateTime(2024, 3, 22), TotalDays = 3, LeaveType = LeaveTypeEnum.UnpaidLeave, Reason = "Emergency", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 18) },

                // Leaves for Employee 5 (Raj)
                new Leave { LeaveId = 13, EmployeeId = 5, StartDate = new DateTime(2024, 3, 3), EndDate = new DateTime(2024, 3, 5), TotalDays = 3, LeaveType = LeaveTypeEnum.SickLeave, Reason = "Cold", Status = StatusEnum.Approved, AppliedAt = new DateTime(2024, 3, 1) },
                new Leave { LeaveId = 14, EmployeeId = 5, StartDate = new DateTime(2024, 3, 10), EndDate = new DateTime(2024, 3, 12), TotalDays = 3, LeaveType = LeaveTypeEnum.SickLeave, Reason = "Religious event", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 7) },
                new Leave { LeaveId = 15, EmployeeId = 5, StartDate = new DateTime(2024, 3, 18), EndDate = new DateTime(2024, 3, 20), TotalDays = 3, LeaveType = LeaveTypeEnum.UnpaidLeave, Reason = "Vacation", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 16) },

                // Leaves for Employee 6 (Meera)
                new Leave { LeaveId = 16, EmployeeId = 6, StartDate = new DateTime(2024, 3, 5), EndDate = new DateTime(2024, 3, 7), TotalDays = 3, LeaveType = LeaveTypeEnum.SickLeave, Reason = "Flu", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 3) },
                new Leave { LeaveId = 17, EmployeeId = 6, StartDate = new DateTime(2024, 3, 12), EndDate = new DateTime(2024, 3, 14), TotalDays = 3, LeaveType = LeaveTypeEnum.Vacation, Reason = "Family function", Status = StatusEnum.Approved, AppliedAt = new DateTime(2024, 3, 10) },
                new Leave { LeaveId = 18, EmployeeId = 6, StartDate = new DateTime(2024, 3, 18), EndDate = new DateTime(2024, 3, 20), TotalDays = 3, LeaveType = LeaveTypeEnum.UnpaidLeave, Reason = "Personal development", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 16) },

                // Leaves for Employee 7 (Kunal)
                new Leave { LeaveId = 19, EmployeeId = 7, StartDate = new DateTime(2024, 3, 5), EndDate = new DateTime(2024, 3, 7), TotalDays = 3, LeaveType = LeaveTypeEnum.SickLeave, Reason = "Cold", Status = StatusEnum.Approved, AppliedAt = new DateTime(2024, 3, 3) },
                new Leave { LeaveId = 20, EmployeeId = 7, StartDate = new DateTime(2024, 3, 14), EndDate = new DateTime(2024, 3, 16), TotalDays = 3, LeaveType = LeaveTypeEnum.Vacation, Reason = "Vacation", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 12) },
                new Leave { LeaveId = 21, EmployeeId = 7, StartDate = new DateTime(2024, 3, 22), EndDate = new DateTime(2024, 3, 24), TotalDays = 3, LeaveType = LeaveTypeEnum.UnpaidLeave, Reason = "Personal reasons", Status = StatusEnum.Pending, AppliedAt = new DateTime(2024, 3, 20) }
            };

        }

    }
}