using System;
using System.Collections.Generic;
using System.Text;

namespace GeoApp
{
    public class MetadataXamlLabel
    {
        public string LabelTitle { get; set; }
        public string KeyboardEntryType { get; set; }

        public MetadataXamlLabel() { }

        public MetadataXamlLabel(string labelTitle, string keyboardEntryType) {
            this.LabelTitle = labelTitle;
            this.KeyboardEntryType = keyboardEntryType;
        }
    }
}
