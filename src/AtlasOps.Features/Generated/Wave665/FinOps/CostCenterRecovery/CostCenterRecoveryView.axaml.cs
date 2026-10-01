namespace AtlasOps.Features.FinOps.CostCenterRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostCenterRecoveryView : UserControl
{
    public CostCenterRecoveryView()
    {
        this.DataContext = new CostCenterRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostCenterRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}