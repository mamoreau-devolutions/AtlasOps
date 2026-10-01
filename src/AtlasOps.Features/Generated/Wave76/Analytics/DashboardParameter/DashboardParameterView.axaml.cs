namespace AtlasOps.Features.Analytics.DashboardParameter;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DashboardParameterView : UserControl
{
    public DashboardParameterView()
    {
        this.DataContext = new DashboardParameterViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DashboardParameterViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}