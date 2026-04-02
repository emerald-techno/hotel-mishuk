using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data;

namespace Persistence.ContextModel;

public interface IApplicationDbContext
{
    public IDbConnection Connection { get; }
    DatabaseFacade Database { get; }
}
