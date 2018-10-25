﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoApp {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExistingDetailFormView : ContentPage {
        public ExistingDetailFormView(Feature data) {
            InitializeComponent();
            ((DetailFormViewModel)BindingContext).EntryID = data.Properties.Id;

            Title = $"View {data.Geometry.Type}";
            itemName.Text = data.Properties.Name;
            dateEntry.Text = data.Properties.Date.ToShortDateString();

            // fill in geo-location data
            geolocationListView.ItemsSource = data.Geometry.XamarinCoordinates;
            // assign metadatefileds as itemsource
            listView.ItemsSource = data.Properties.MetadataFields;
        }

        private void listView_ItemTapped(object sender, ItemTappedEventArgs e) {
            geolocationListView.SelectedItem = null;
            listView.SelectedItem = null;
        }
    }
}