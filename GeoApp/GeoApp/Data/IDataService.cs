using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoApp
{
    public interface IDataService
    {
        Task<List<Properties>> RefreshDataAsync();

        Task SaveLocationAsync(Properties location);

        Task DeleteLocationAsync(string Name);
        Task DeleteLocationAsync(int id);
    }
}
