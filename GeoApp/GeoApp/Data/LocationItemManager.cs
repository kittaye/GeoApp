using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeoApp.Data
{
    public class LocationItemManager
    {
        IDataService restService;

        public List<RootObject> CurrentLocations { get; set; }

        public LocationItemManager(IDataService service)
        {
            restService = service;
        }

        public Task<List<Feature>> GetLocationsAsync()
        {
            return restService.RefreshDataAsync();
        }

        public Task SaveLocationAsync(RootObject location)
        {
            return restService.SaveLocationAsync(location);
        }

        public Task DeleteLocationAsync(RootObject location)
        {
            return restService.DeleteLocationAsync(location.features[0].properties.id);
        }
    }
}

