using WebMVC.Models.IdentityModels;

namespace WebMVC.Models.UserRole;

public class RolePermissionHelperOld
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
                        Id = childId++, Title = "Fee Module", ParentId = parentId,
                        IsSelected = false, Name = Permissions.Module.FeeModule
                    },
                    new RolePermmissionCheck {
                        Id = childId++, Title = "Academic Module", ParentId = parentId,
                        IsSelected = false, Name = Permissions.Module.AcademicModule
                    },
                    new RolePermmissionCheck {
                        Id = childId++, Title = "Accounts Module", ParentId = parentId,
                        IsSelected = false, Name = Permissions.Module.AccountsModule
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
                }
            },

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

            // Settings...
            new RolePermmissionCheck {
                Id = (++parentId), Title="Settings", IsSelected = false, Children = new List<RolePermmissionCheck> {
                    new RolePermmissionCheck {
                        Id = childId++, Title = "List View", ParentId = parentId,
                        IsSelected = false, Name = Permissions.Settings.ListView
                    },
                    new RolePermmissionCheck {
                        Id = childId++, Title = "Create", ParentId = parentId,
                        IsSelected = false, Name = Permissions.Settings.Create
                    },
                    new RolePermmissionCheck {
                        Id = childId++, Title = "Edit", ParentId = parentId,
                        IsSelected = false, Name = Permissions.Settings.Edit
                    },
                    new RolePermmissionCheck {
                        Id = childId++, Title = "Delete", ParentId = parentId,
                        IsSelected = false, Name = Permissions.Settings.Delete
                    }
                }
            },

            //System Settings
            new RolePermmissionCheck {
                Id = (++parentId), Title="System Settings", IsSelected = false, Children = new List<RolePermmissionCheck> {
                    new RolePermmissionCheck {
                        Id = childId++, Title = "BackupDB", ParentId = parentId,
                        IsSelected = false, Name = Permissions.SystemSettings.BackupDB
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
                            Id = childId++, Title = "Create Academic Department", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Departments.CreateAcademicDepartment
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Departments.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit Academic Department", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Departments.EditAcademicDepartment
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Departments.Delete
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete Academic Department", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Departments.DeleteAcademicDepartment
                        }
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

                // Set Room Categories...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Set Room Categories", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomCategories.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomCategories.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomCategories.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomCategories.Delete
                        }
                    }
                },

                // Set Room Info...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Set Room Info", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomInfos.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomInfos.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomInfos.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Room Assign", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomInfos.RoomAssign
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomInfos.Details
                        }
                    }
                },

                // Set Room Facilities...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Set Room Facilities", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomFacilities.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomFacilities.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomFacilities.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomFacilities.Delete
                        }
                    }
                },

                // Set Room Facility Categories...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Set Room Facility Category", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomFacilityCategories.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomFacilityCategories.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomFacilityCategories.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomFacilityCategories.Delete
                        }
                    }
                },

                // Set Floor Infos...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Set Floor", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.FloorInfos.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.FloorInfos.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.FloorInfos.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.FloorInfos.Delete
                        }
                    }
                },

                // Set Task Type...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Set Task Type", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.TaskTypes.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.TaskTypes.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.TaskTypes.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.TaskTypes.Delete
                        },
                    }
                },

                // Set Customer...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Set Customer", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Customers.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Customers.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Customers.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Customers.Delete
                        },
                    }
                },

                // Set Task Name...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Set Task Name", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.TaskNames.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.TaskNames.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.TaskNames.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.TaskNames.Delete
                        },
                    }
                },

                // Room Assign...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Room Assign", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomAssigns.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomAssigns.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "AssignRoom", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomAssigns.AssignRoom
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "UnassignRoom", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomAssigns.UnassignRoom
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "AssignSingleRoom", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomAssigns.AssignSingleRoom
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "UnassignSingleRoom", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomAssigns.UnassignSingleRoom
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "CleaningStatusChange", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomAssigns.CleaningStatusChange
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "AvailabilityStatusChange", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomAssigns.AvailabilityStatusChange
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "GetAssignRooms", ParentId = parentId,
                            IsSelected = false, Name = Permissions.RoomAssigns.GetAssignRooms
                        }
                    }
                },

                // Task Assign...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Task Assign", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.TaskAssigns.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.TaskAssigns.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "My Task", ParentId = parentId,
                            IsSelected = false, Name = Permissions.TaskAssigns.MyTask
                        }
                    }
                },

                 // Set Food Categories...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Food Category", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.FoodCategories.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.FoodCategories.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.FoodCategories.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.FoodCategories.Delete
                        }
                    }
                },

                // Set Bed Types...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Bed Type", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.BedTypes.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.BedTypes.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.BedTypes.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.BedTypes.Delete
                        }
                    }
                },

                // Set Complementary...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Complementary", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Complementary.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Complementary.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Complementary.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Complementary.Delete
                        }
                    }
                },

                // Set Service...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Service", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Services.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Services.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Services.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Services.Delete
                        }
                    }
                },

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
                    Id = (++parentId), Title="Item Info", IsSelected = false, Children = new List<RolePermmissionCheck> {
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

                // Set Requisition Info...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Receive", IsSelected = false, Children = new List<RolePermmissionCheck> {
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

                // Set Inventory Report...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="InventoryReport", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.InventoryReport.View
                        }
                    }
                },

                // Set Guest Info...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Guest Info", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.GuestInfos.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.GuestInfos.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Edit", ParentId = parentId,
                            IsSelected = false, Name = Permissions.GuestInfos.Edit
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.GuestInfos.Delete
                        }
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

                // Set Bills...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Bill", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Bills.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Bills.Details
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "PayBills", ParentId = parentId,
                            IsSelected = false, Name = Permissions.Bills.PayBills
                        }
                    }
                },

                // Set Booking Service...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Booking Service", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.BookingServices.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Details", ParentId = parentId,
                            IsSelected = false, Name = Permissions.BookingServices.Details
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.BookingServices.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "CheckIn", ParentId = parentId,
                            IsSelected = false, Name = Permissions.BookingServices.CheckIn
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "CheckOut", ParentId = parentId,
                            IsSelected = false, Name = Permissions.BookingServices.CheckOut
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Payment", ParentId = parentId,
                            IsSelected = false, Name = Permissions.BookingServices.Payment
                        },
                    }
                },

                // Set Online Booking Service...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Online Booking", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "List View", ParentId = parentId,
                            IsSelected = false, Name = Permissions.OnlineBooking.ListView
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Approve", ParentId = parentId,
                            IsSelected = false, Name = Permissions.OnlineBooking.Approve
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Reject", ParentId = parentId,
                            IsSelected = false, Name = Permissions.OnlineBooking.Reject
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.OnlineBooking.Delete
                        },
                    }
                },

                // Set Emp Disciplinary...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Employee Disciplinary", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpDisciplinary.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Get Employee Discipline", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpDisciplinary.GetEmployeeDiscipline
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpDisciplinary.Delete
                        },
                    }
                },

                // Set Emp Education...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Employee Education", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpEducation.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Get Employee Education", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpEducation.GetEmployeeEducation
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpEducation.Delete
                        },
                    }
                },

                // Set Emp Experience...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Employee Experience", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpExperience.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Get Employee Experience", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpExperience.GetEmployeeExperience
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpExperience.Delete
                        },
                    }
                },

                // Set Emp Journal...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Employee Journal", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpJournal.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Get Employee Experience", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpJournal.GetEmployeeJournal
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpJournal.Delete
                        },
                    }
                },

                // Set Emp Posting...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Employee Posting", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpPosting.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Get Employee Posting", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpPosting.GetEmployeePosting
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpPosting.Delete
                        },
                    }
                },

                // Set Emp Referance...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Employee Referance", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpReference.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Get Employee Referance", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpReference.GetEmployeeReference
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpReference.Delete
                        },
                    }
                },

                // Set Emp Training...
                new RolePermmissionCheck {
                    Id = (++parentId), Title="Employee Training", IsSelected = false, Children = new List<RolePermmissionCheck> {
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Create", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpTraining.Create
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Get Employee Training", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpTraining.GetEmployeeTraining
                        },
                        new RolePermmissionCheck {
                            Id = childId++, Title = "Delete", ParentId = parentId,
                            IsSelected = false, Name = Permissions.EmpTraining.Delete
                        },
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
