namespace AtlasOps.Features.Observability.ObservabilitySloOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilitySloOptimizationView : UserControl
{
    public ObservabilitySloOptimizationView()
    {
        this.DataContext = new ObservabilitySloOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilitySloOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}