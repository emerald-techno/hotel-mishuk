using System;

namespace Domain.ModelInterface;

public interface IDeleteable
{
    bool IsDeleted { get; set; }
    long? DeletedById { get; set; }
    DateTime? DeletedOn { get; set; }
    void Delete(long? deletedById = null);
    //AppIdentityUser DeletedBy { get; set; }
}
