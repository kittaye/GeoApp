using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GeoApp {
    public class ListViewItemTappedBehaviour : Behavior<ListView> {

        public static readonly BindableProperty CommandProperty =
                BindableProperty.Create("Command", typeof(ICommand), typeof(ListViewItemTappedBehaviour), null);
        public static readonly BindableProperty InputConverterProperty =
                BindableProperty.Create("Converter", typeof(IValueConverter), typeof(ListViewItemTappedBehaviour), null);

        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public IValueConverter Converter {
            get { return (IValueConverter)GetValue(InputConverterProperty); }
            set { SetValue(InputConverterProperty, value); }
        }

        public ListView AssociatedObject { get; private set; }

        /// <summary>
        /// Override method used to wire up the ItemTapped event handler 
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnAttachedTo(ListView bindable) {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;
            bindable.BindingContextChanged += OnBindingContextChanged;
            bindable.ItemTapped += OnListViewItemTapped;
        }

        /// <summary>
        /// Override method occurs when behaviour is removed from UI control and clean up
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnDetachingFrom(ListView bindable) {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            bindable.ItemTapped -= OnListViewItemTapped;
            AssociatedObject = null;
        }

        void OnBindingContextChanged(object sender, EventArgs e) {
            OnBindingContextChanged();
        }

        void OnListViewItemTapped(object sender, ItemTappedEventArgs e) {
            if (Command == null) {
                return;
            }
            AssociatedObject.SelectedItem = null;

            object parameter = Converter.Convert(e, typeof(object), null, null);

            if (Command.CanExecute(parameter)) {
                Command.Execute(parameter);
            }
        }

        protected override void OnBindingContextChanged() {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }
    }
}
