﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PCLStorage;

namespace GeoApp {
    public interface IDataService {
        Task<List<Feature>> RefreshDataAsync();
        Task<bool> SaveFeatureAsync(Feature feature);
        Task SaveAllCurrentFeaturesAsync();
        Task<bool> DeleteFeatureAsync(int id);
        Task DeleteAllFeaturesAsync();
        Task<bool> ImportFeaturesFromFile(string path);
        Task<bool> ImportFeaturesAsync(string fileContents);
        string ExportFeaturesToJson();
        Task<IFile> GetEmbeddedFile();
        Task<IFile> GetLogFile();
    }
}
