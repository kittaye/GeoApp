using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace GeoApp
{
    /// <summary>
    /// Defines the model for metadata entries in a listview.
    /// </summary>
    public class MetadataEntry : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public string _labelTitle { get; set; }
        public string LabelTitle {
            get { return _labelTitle; }
            set { _labelTitle = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LabelTitle")); }
        }

        public string _labelData { get; set; }
        public string LabelData {
            get { return _labelData; }
            set { _labelData = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LabelData")); }
        }

        public Keyboard EntryType { get; set; }


        public MetadataEntry() { }

        public MetadataEntry(string labelTitle, string labelData, Keyboard entryType) {
            this.LabelTitle = labelTitle;
            this.LabelData = labelData;
            this.EntryType = entryType;
        }
    }
}
