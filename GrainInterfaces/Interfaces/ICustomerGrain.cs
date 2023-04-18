using States.Models;

namespace GrainInterfaces.Interfaces
{
    public interface ICustomerGrain : IGrainWithIntegerKey
    {
        Task<CustomerState> Get();
        Task Set(CustomerState customer);
    }
}
