namespace AtlasOps.Features.Analytics.DashboardLayout;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DashboardLayoutView : UserControl
{
    public DashboardLayoutView()
    {
        this.DataContext = new DashboardLayoutViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DashboardLayoutViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}