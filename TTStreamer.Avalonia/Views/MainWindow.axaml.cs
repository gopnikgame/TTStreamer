using System.Threading;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using TTStreamer.Models;

namespace TTStreamer.Views
{
    public partial class MainWindow : Window
    {

        private ScrollViewer _scrollViewer;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainView viewModel)
        {
            this.Closed += MainWindow_Closed;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void MainWindow_Closed(object? sender, System.EventArgs e)
        {
            (DataContext as MainView).SaveState();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _scrollViewer = this.FindControl<ScrollViewer>("giftsSV");
        }

        private void ShowPopup(object? sender, RoutedEventArgs e)
        {
            var window = App.GetService<SoundsWindow>();
            window.ShowDialog(this);
        }

        private void AvaloniaObject_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name.Equals(ItemsControl.ItemCountProperty.Name) && _scrollViewer != null && ((MainView)DataContext).AutoScroll)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(30);
                    Dispatcher.UIThread.InvokeAsync(_scrollViewer.ScrollToEnd);
                });
            }
        }
    }
}