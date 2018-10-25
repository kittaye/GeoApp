using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoApp
{
    public interface IDataService
    {
        Task<List<Feature>> RefreshDataAsync();
        Task SaveLocationAsync(Feature location);

        Task DeleteLocationAsync(string Name);
        Task DeleteLocationAsync(int id);
        void AddLocationsFromFile(string path);
    }
}
