using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoApp {
    public interface IDataService {
        Task<List<Feature>> RefreshDataAsync();
        Task<bool> SaveLocationAsync(Feature location);
        Task<bool> DeleteLocationAsync(int id);
        Task<bool> ImportLocationsFromFile(string path);
        Task<bool> ImportLocationsAsync(string fileContents);
        string ExportLocationsToJson();
    }
}
