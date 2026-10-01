namespace AtlasOps.Features.Compute.ComputeScaleSetRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeScaleSetRecoveryView : UserControl
{
    public ComputeScaleSetRecoveryView()
    {
        this.DataContext = new ComputeScaleSetRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeScaleSetRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}