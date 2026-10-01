namespace AtlasOps.Features.FinOps.CostAnomalyRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostAnomalyRecoveryView : UserControl
{
    public CostAnomalyRecoveryView()
    {
        this.DataContext = new CostAnomalyRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostAnomalyRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}