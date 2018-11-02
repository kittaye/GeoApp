using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoApp {
    public interface IDataService {
        Task<List<Feature>> RefreshDataAsync();
        Task<bool> SaveFeatureAsync(Feature feature);
        Task<bool> DeleteFeatureAsync(int id);
        Task<bool> ImportFeaturesFromFile(string path);
        Task<bool> ImportFeaturesAsync(string fileContents);
        string ExportFeaturesToJson();
    }
}
