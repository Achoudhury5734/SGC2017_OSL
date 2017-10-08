using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSL
{
    public interface IDataStore<T>
    {
        Task<bool> AddPickupItemAsync(T item);
        Task<bool> UpdatePickupItemAsync(T item);
        Task<bool> DeletePickupItemAsync(string id);
        Task<T> GetPickupItemAsync(string id);
        Task<IEnumerable<T>> GetPickupItemsAsync(bool forceRefresh = false);
    }
}
