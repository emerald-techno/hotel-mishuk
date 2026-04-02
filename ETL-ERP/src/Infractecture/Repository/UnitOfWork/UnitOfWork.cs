using Interface.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.ContextModel;
using System.Security.Claims;

namespace Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {

        #region Config

        private readonly ApplicationDbContext _context;
        public long CurrentUserId { get; set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            this._context = context;
        }

        public UnitOfWork(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpAccessor)
        {
            _context = applicationDbContext;
            var id = httpAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(id)) id = httpAccessor.HttpContext.User?.Claims.FirstOrDefault(c => c.Type.Contains("sub"))?.Value;
            CurrentUserId = !string.IsNullOrEmpty(id) ? Convert.ToInt64(id) : 0;
            _context.CurrentUserId = CurrentUserId;
        }

        #endregion

        public Task<IDbContextTransaction> BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public bool Complete()
        {
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> CompleteAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
