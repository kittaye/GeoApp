using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GeoApp
{
    public class MetadataEntry
    {
        public string LabelTitle { get; set; }
        public Keyboard EntryType { get; set; }

        public MetadataEntry() { }

        public MetadataEntry(string labelTitle, Keyboard entryType) {
            this.LabelTitle = labelTitle;
            this.EntryType = entryType;
        }
    }
}
