using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.Customer;
using Interface.Repository.Restaurant;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Restaurant;

public class CustomerRepository : BaseRepository<RsCustomer>, ICustomerRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public CustomerRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }

    #endregion

    public async Task<DataTablePagination<CustomerSearchVm, CustomerSearchVm>> SearchAsync(DataTablePagination<CustomerSearchVm, CustomerSearchVm> vm)
    {
        var model = vm.SearchModel;
        var searchResult = Context.RsCustomers
                           .Include(c=>c.Company)
                           .Include(c=>c.Guest)
                           .Include(c=>c.Employee)
                          .AsQueryable().Where(c => !c.IsDeleted);

        if (model == null) throw new Exception("Search customer info not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => (c.Salutation + " " + c.FirstName + " " + c.LastName).ToLower().Contains(value) || c.Mobile.ToLower().Contains(value));
        }
        if (model.CompanyId > 0)
        {
            searchResult = searchResult.Where(c => c.CompanyId == model.CompanyId);
        }

        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.FirstName)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<CustomerSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.CompanyName = filterData.Company?.Name;

                var salutation = !string.IsNullOrEmpty(filterData.Salutation) ? filterData.Salutation : "";
                var firstName = !string.IsNullOrEmpty(filterData.FirstName) ? filterData.FirstName : "";
                var lastName = !string.IsNullOrEmpty(filterData.LastName) ? filterData.LastName : "";

                searchDto.FullName = $"{salutation} {firstName} {lastName}";

                var guestSalutation = !string.IsNullOrEmpty(filterData.Guest?.Salutation) ? filterData.Guest?.Salutation : "";
                var guestFirstName = !string.IsNullOrEmpty(filterData.Guest?.FirstName) ? filterData.Guest?.FirstName : "";
                var guestLastName = !string.IsNullOrEmpty(filterData.Guest?.LastName) ? filterData.Guest?.LastName : "";

                searchDto.GuestName = $"{guestSalutation} {guestFirstName} {guestLastName}";

                searchDto.EmployeeName = filterData.Employee?.Name;
            }
        }
        return vm;
    }

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    #endregion
}
