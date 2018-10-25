using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoApp {
    public interface IDataService {
        Task<List<Feature>> RefreshDataAsync();
        Task SaveLocationAsync(Feature location);
        Task EditSaveLocationAsync(Feature location);
        Task<bool> DeleteLocationAsync(int id);
        Task AddLocationsFromFile(string path);
        Task ImportLocationsAsync(string fileContents);
        Task<string> ExportLocationsAsync();
    }
}
