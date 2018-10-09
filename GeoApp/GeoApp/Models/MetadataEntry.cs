using System;
using System.Collections.Generic;
using System.Text;

namespace GeoApp
{
    public enum MetaDataTypes { String, Integer, Float }
    public class MetadataEntry
    {
        public string LabelTitle { get; set; }
        public MetaDataTypes EntryType { get; set; }

        public MetadataEntry() { }

        public MetadataEntry(string labelTitle, MetaDataTypes entryType) {
            this.LabelTitle = labelTitle;
            this.EntryType = entryType;
        }
    }
}
