﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace GeoApp.Data {
    public class FeaturesManager {
        IDataService restService;

        public List<Feature> CurrentFeatures { get; set; } = new List<Feature>();

        public FeaturesManager(IDataService service) {
            restService = service;
        }

        public Task<List<Feature>> GetFeaturesAsync() {
            return restService.RefreshDataAsync();
        }

        public Task SaveFeatureAsync(Feature feature) {
            return restService.SaveFeatureAsync(feature);
        }

        public Task SaveAllCurrentFeaturesAsync() {
            return restService.SaveAllCurrentFeaturesAsync();
        }

        public Task<bool> DeleteFeatureAsync(int id)
        {
            return restService.DeleteFeatureAsync(id);
        }

        public void ImportFeaturesFromFile(string path) {
            restService.ImportFeaturesFromFile(path);
        }

        public async Task ImportFeaturesAsync(string fileContents) {
            await restService.ImportFeaturesAsync(fileContents);
        }

        public async Task<IFile> GetEmbeddedFile()
        {
            return await restService.GetEmbeddedFile();
        }

        public string ExportFeaturesToJson() {
            return restService.ExportFeaturesToJson();
        }
    }
}

