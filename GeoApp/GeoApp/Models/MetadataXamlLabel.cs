using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GeoApp
{
    public class MetadataXamlLabel
    {
        public string LabelTitle { get; set; }
        public Keyboard KeyboardEntryType { get; set; }

        public MetadataXamlLabel() { }

        public MetadataXamlLabel(string labelTitle, Keyboard keyboardEntryType) {
            this.LabelTitle = labelTitle;
            this.KeyboardEntryType = keyboardEntryType;
        }
    }
}
