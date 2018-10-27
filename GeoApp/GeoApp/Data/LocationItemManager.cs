using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeoApp.Data {
    public class LocationItemManager {
        IDataService restService;

        public List<Feature> CurrentLocations { get; set; } = new List<Feature>();

        public LocationItemManager(IDataService service) {
            restService = service;
        }

        public Task<List<Feature>> GetLocationsAsync() {
            return restService.RefreshDataAsync();
        }

        public Task SaveLocationAsync(Feature location) {
            return restService.SaveLocationAsync(location);
        }

        public Task<bool> DeleteLocationAsync(int id)
        {
            return restService.DeleteLocationAsync(id);
        }

        public void AddLocationShare(string path) {
            restService.AddLocationsFromFile(path);
        }

        public async Task ImportLocationsAsync(string fileContents) {
            await restService.ImportLocationsAsync(fileContents);
        }

        public string ExportLocationsToJson() {
            return restService.ExportLocationsToJson();
        }
    }
}

