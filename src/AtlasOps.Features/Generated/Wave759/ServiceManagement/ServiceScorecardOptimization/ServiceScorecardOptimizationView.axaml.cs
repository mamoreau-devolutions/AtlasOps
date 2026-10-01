namespace AtlasOps.Features.ServiceManagement.ServiceScorecardOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceScorecardOptimizationView : UserControl
{
    public ServiceScorecardOptimizationView()
    {
        this.DataContext = new ServiceScorecardOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceScorecardOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}