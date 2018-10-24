using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeoApp.Data
{
    public class LocationItemManager
    {
        IDataService restService;

        public List<Feature> CurrentLocations { get; set; }

        public LocationItemManager(IDataService service)
        {
            restService = service;
        }

        public Task<List<Feature>> GetLocationsAsync()
        {
            return restService.RefreshDataAsync();
        }
        public Task CreateFile()
        {
            return restService.CreateFile();
        }

        public Task SaveLocationAsync(Feature location)
        {
            return restService.SaveLocationAsync(location);
        }

        public Task DeleteLocationAsync(RootObject location)
        {
            return restService.DeleteLocationAsync(location.Features[0].Properties.Id);
        }
    }
}

