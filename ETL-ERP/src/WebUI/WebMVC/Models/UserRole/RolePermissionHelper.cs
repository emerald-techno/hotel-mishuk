using WebMVC.Models.IdentityModels;

namespace WebMVC.Models.UserRole
{
    public class RolePermissionHelper
    {
        #region setup role permission for view
        private int parentId { get; set; } = 1;
        private int childId { get; set; } = 100;

        public IList<RolePermmissionCheck> Items { get; set; }

        public void LoadItems()
        {
            this.Items = new List<RolePermmissionCheck> {

                // Module...
                new RolePermmissionCheck {
                    Id = parentId, Title = "Module", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Admin Module", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Module.AdminModule
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "HR Module", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Module.HrModule
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Leave Module", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Module.LeaveModule
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Payroll Module", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Module.PayrollModule
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "PF Module", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Module.PfModule
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Accounts Module", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Module.AccountsModule
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Inventory Module", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Module.InventoryModule
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Hotel Management Module", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Module.HotelManagementModule
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "House Keeping Module", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Module.HouseKeepingModule
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Restaurant Module", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Module.RestaurantModule
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "MIS Module", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Module.MisModule
                        },
                    }
                },

                // --- Admin Module ---

                // User...
                new RolePermmissionCheck {
                    Id = parentId, Title = "Users", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Users.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Users.Create
                        },
                    }
                },

                // User Role...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="User Roles", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.UserRoles.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create Or Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.UserRoles.CreateOrEdit
                        },
                    }
                },

                // Department...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Department", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Departments.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View Academic Department", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Departments.ListViewAcademicDepartment
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Departments.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Departments.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Departments.Delete
                        },
                    }
                },

                // Designations...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Designations", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Designations.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Designations.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Designations.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Designations.Delete
                        }
                    }
                },

                // Financial Year...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Financial Year", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetFincYear.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetFincYear.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetFincYear.Delete
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetFincYear.ListView
                        },
                    }
                },

                // Set Duty Shift...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Duty Shift", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.DutyShifts.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.DutyShifts.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.DutyShifts.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.DutyShifts.Delete
                        }
                    }
                },

                // Set Shift Management...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Shift Management", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.ShiftManagements.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.ShiftManagements.Create
                        }
                    }
                },

                // Office Settings...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Office Settings", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Setup View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.HrSettings.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details", ParentId = parentId,
                            IsSelected = false, Name = Permissions.HrSettings.DetailsView
                        }
                    }
                },

                // --- HR Module ---

                // Employees... 
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Employees", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Employees.Create
                        },

                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Employees.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Employees.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Employees.DetailsView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Report View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Employees.ReportView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Disable", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Employees.Disable
                        },
                    }
                },

                // Employee Attendances...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Employee Attendance", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpAttendances.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Report View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpAttendances.ReportView
                        }
                    }
                },

                // --- Leave Module ---

                // Leave Type...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Leave Type", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.LeaveTypes.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.LeaveTypes.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.LeaveTypes.Delete
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.LeaveTypes.ListView
                        }
                    }
                },

                // Leave Setup...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Leave Setup", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.LeaveSetups.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.LeaveSetups.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.LeaveSetups.Delete
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.LeaveSetups.ListView
                        }
                    }
                },

                // Cf Leave...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="CF Leave", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.CfLeave.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.CfLeave.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.CfLeave.Delete
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.CfLeave.ListView
                        }
                    }
                },

                // Set Holidays...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Set Holidays", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetHolidays.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetHolidays.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetHolidays.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetHolidays.Delete
                        }
                    }
                },

                // Employee Leave Applications...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Employee Leave Application", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpLeaveApplications.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpLeaveApplications.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpLeaveApplications.Delete
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpLeaveApplications.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Report View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpLeaveApplications.ReportView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpLeaveApplications.DetailsView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Statement View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpLeaveApplications.StatementView
                        }
                    }
                },

                // --- Payroll Module ---

                // Set Salary Grades...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Set Salary Grades", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetSalaryGrades.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetSalaryGrades.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetSalaryGrades.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetSalaryGrades.Delete
                        }
                    }
                },

                // Payroll Salary Part...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Payroll Salary Part", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrSalaryParts.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrSalaryParts.ListView
                        }
                    }
                },

                // Payroll Employee Salary Part...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Payroll Employee Salary Part", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrEmpSalaryParts.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrEmpSalaryParts.DetailsView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrEmpSalaryParts.ListView
                        }
                    }
                },

                // Monthly Attendance Sheet...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Monthly Attendance Sheet", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.MonthlyAttendanceSheetMsts.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.MonthlyAttendanceSheetMsts.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.MonthlyAttendanceSheetMsts.DetailsView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Report View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.MonthlyAttendanceSheetMsts.ReportView
                        }
                    }
                },

                // Payroll Arrear...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Pr Arrear", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrArrearMsts.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrArrearMsts.Delete
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrArrearMsts.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrArrearMsts.DetailsView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Approve", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrArrearMsts.Approve
                        }
                    }
                },

                // Employee Loan...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Employee Loan", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpLoanMsts.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpLoanMsts.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpLoanMsts.DetailsView
                        }
                    }
                },

                // Payroll Guest Salary...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Payroll Guest Salary", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrGuestSalaryMsts.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrGuestSalaryMsts.DetailsView
                        },
                         new RolePermmissionCheck {
                            Id = childId++, Title = "Approve", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrGuestSalaryMsts.Approve
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrGuestSalaryMsts.ListView
                        }
                    }
                },

                // Payroll Salary...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Payroll Salary", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "SetPayroll", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrSalaryMsts.SetPayroll
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrSalaryMsts.DetailsView
                        },
                         new RolePermmissionCheck {
                            Id = childId++, Title = "Payroll", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrSalaryMsts.Payroll
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrSalaryMsts.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Payslip", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrSalaryMsts.Payslip
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "PayMultiSalary", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrSalaryMsts.PayMultiSalary
                        },
                         new RolePermmissionCheck {
                            Id = childId++, Title = "Approve", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrSalaryMsts.Approve
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "PayPayslip", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PrSalaryMsts.PayPayslip
                        }
                    }
                },

                // --- PF Module ---

                // Pf Settings...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Pf Settings", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PfSettings.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PfSettings.DetailsView
                        }
                    }
                },

                // Pf Fund Openning...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Pf Fund Openning", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PfFundOpennings.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PfFundOpennings.ListView
                        }
                    }
                },

                // Pf Settlements...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Pf Settlements", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PfSettlements.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PfSettlements.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PfSettlements.ListView
                        }
                    }
                },

                // Pf Fund Mst...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Pf Fund Mst", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "ReportGenerate", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PfFundMsts.ReportGenerate
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PfFundMsts.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Report View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PfFundMsts.ReportView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.PfFundMsts.DetailsView
                        },
                    }
                },

                // --- Accounting Module ---

                // Set Currencies...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Set Currencies", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetCurrencies.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetCurrencies.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetCurrencies.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SetCurrencies.Delete
                        }
                    }
                },

                // Account Groups...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Account Groups", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccGroups.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccGroups.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccGroups.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccGroups.Delete
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Chart Of Accounts", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccGroups.ChartOfAcc
                        }
                    }
                },

                // Account Heads...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Account Heads", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccHeads.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccHeads.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccHeads.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccHeads.Delete
                        }
                    }
                },

                // Account Ledgers...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Account Ledgers", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccLedgers.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccLedgers.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccLedgers.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccLedgers.Delete
                        }
                    }
                },

                // Account Transactions...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Account Transactions", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccTranMsts.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccTranMsts.DetailsView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Journal Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccTranMsts.JournalCreate
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Account Opening", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccTranMsts.AccOpeningCreate
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Bank Debit Voucher", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccTranMsts.BankDebitVoucher
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Bank Credit Voucher", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccTranMsts.BankCreditVoucher
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Cash Debit Voucher", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccTranMsts.CashDebitVoucher
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Cash Credit Voucher", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccTranMsts.CashCreditVoucher
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccTranMsts.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Report View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccTranMsts.ReportView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Voucher Search", ParentId = parentId,
                            IsSelected = false, Name = Permissions.AccTranMsts.VoucherSearch
                        }

                    }
                },

                //--- Inventory Module ----

                // Set Category Info...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Category Info", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.CategoryInfo.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.CategoryInfo.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.CategoryInfo.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.CategoryInfo.Delete
                        }
                    }
                },

                // Set Item Info...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Item Info", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.ItemInfo.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.ItemInfo.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.ItemInfo.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.ItemInfo.Delete
                        }
                    }
                },

                // Set Unit Info...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Unit Info", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.UnitInfo.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.UnitInfo.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.UnitInfo.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.UnitInfo.Delete
                        }
                    }
                },

                // Set Supplier Info...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Supplier Info", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SupplierInfo.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SupplierInfo.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SupplierInfo.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SupplierInfo.Delete
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Detail", ParentId = parentId,
                            IsSelected = false, Name = Permissions.SupplierInfo.Detail
                        }
                    }
                },

                // Set Requisition Info...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Requisition", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Requisition.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Requisition.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Detail", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Requisition.Detail
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Department Wise Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Requisition.DepartmentWiseCreate
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Department Wise List", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Requisition.DepartmentWiseList
                        }
                    }
                },

                // Set Order...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Order", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Order.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Order.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Detail", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Order.Detail
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Order Pay Bill", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Order.PayBill
                        }
                    }
                },

                // Set Receive...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Receive", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Receive.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Receive.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Detail", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Receive.Detail
                        }
                    }
                },               

                // Set Issue...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Issue", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Issue.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Issue.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Detail", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Issue.Detail
                        }
                    }
                },

                // Set Consumption...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Consumption", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Consumption.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Consumption.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Detail", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Consumption.Detail
                        }
                    }
                },

                // Set Inventory Report...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="InventoryReport", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.InventoryReport.View
                        }
                    }
                },
                //Front Office
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Front-Office", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Reservation Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.HotelManagement.Booking
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Reservation Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.HotelManagement.BookingUpdate
                        }
                    }
                },
        };
    }
    #endregion

        #region role permission helper method

        public void LoadRolePermissions(IList<string> permissions)
        {
            if (this.Items == null || !this.Items.Any() || permissions == null || !permissions.Any())
            {
                return;
            }

            foreach (var root in this.Items)
            {
                var isRootSelect = true;
                foreach (var child in root.Children)
                {
                    if (permissions.Any(p => p == child.Name)) child.IsSelected = true;
                    if (!child.IsSelected) isRootSelect = false;
                }
                root.IsSelected = isRootSelect;
            }
        }

        public void ResetRolePermissions()
        {
            if (this.Items == null || !this.Items.Any())
            {
                return;
            }

            foreach (var root in this.Items)
            {
                root.IsSelected = false;
                foreach (var child in root.Children)
                {
                    child.IsSelected = false;
                }
            }
        }

        public IList<string> PreparePermissions()
        {
            var result = new List<string>();

            foreach (var root in this.Items)
            {
                foreach (var child in root.Children)
                {
                    if (child.IsSelected) result.Add(child.Name);
                }
            }

            return result;
        }

        #endregion
    }
}
