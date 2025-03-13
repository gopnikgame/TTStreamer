using System;
using System.Collections.Generic;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using TTStreamer.Models;

namespace TTStreamer.Views
{
    public partial class SoundsWindow : Window
    {
        public SoundsWindow() { }

        public SoundsWindow(SoundsView viewModel)
        {
            DataContext = viewModel;
            AvaloniaXamlLoader.Load(this);
        }

        private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count == 0) return;
            var keyValue = (sender as Control).DataContext as KeyValuePair<int, string>? ?? default;
            if (keyValue.Value.Equals(e.AddedItems[0].ToString(), StringComparison.OrdinalIgnoreCase)) return;

            (DataContext as SoundsView).Update(keyValue.Key, e.AddedItems[0].ToString());
        }
    }
}