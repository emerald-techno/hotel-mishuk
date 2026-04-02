using Domain.Entities.Admin;
using Domain.Entities.HotelManagement;
using Persistence.ContextModel;

namespace WebMVC.Models.SeedData;

public static class ProjectSeedData
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.RsCustomerTypes.Any() || context.Departments.Any() || context.Designations.Any())
        {
            // Database has been seeded
            return;
        }

        var customerTypes = new RsCustomerType[]
        {
            new RsCustomerType
            {
                TypeName = "Hotel",
                TypeCode = "Hotel",
                DiscountPercent = 0,
                ActionDate = DateTime.Now,
                IsDeleted = false,
                ActionById = 1
            },
            new RsCustomerType
            {
                TypeName = "Walk-In",
                TypeCode = "Walk-In",
                DiscountPercent = 0,
                ActionDate = DateTime.Now,
                IsDeleted = false,
                ActionById = 1
            },
            new RsCustomerType
            {
                TypeName = "Employee",
                TypeCode = "Employee",
                DiscountPercent = 40,
                ActionDate = DateTime.Now,
                IsDeleted = false,
                ActionById = 1
            },
            new RsCustomerType
            {
                TypeName = "Online",
                TypeCode = "Online",
                DiscountPercent = 0,
                ActionDate = DateTime.Now,
                IsDeleted = false,
                ActionById = 1
            },
        };

        context.RsCustomerTypes.AddRange(customerTypes);
        context.SaveChanges();

        var departments = new Department[]
        {
            new Department
            {
                Name = "Front-Desk",
                Code = "DPT-01",
                ActionDate = DateTime.Now,
                ActionById = 1,
                IsDeleted = false
            },
            new Department
            {
                Name = "HouseKeeper",
                Code = "DPT-02",
                ActionDate = DateTime.Now,
                ActionById = 1,
                IsDeleted = false
            },
            new Department
            {
                Name = "Resturant",
                Code = "DPT-03",
                ActionDate = DateTime.Now,
                ActionById = 1,
                IsDeleted = false
            }
        };

        context.Departments.AddRange(departments);
        context.SaveChanges();

        var designations = new Designation[]
        {
            new Designation
            {
                Name = "HouseKeeper",
                Code = "DES-01",
                IsHouseKeeper = true,
                ActionDate = DateTime.Now,
                ActionById = 1,
                IsDeleted = false
            },
            new Designation
            {
                Name = "Waiter",
                Code = "DES-02",
                ActionDate = DateTime.Now,
                ActionById = 1,
                IsDeleted = false
            }
        };

        context.Designations.AddRange(designations);
        context.SaveChanges();
    }
}

