namespace AtlasOps.Features.Compute.ComputePatchRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputePatchRecoveryView : UserControl
{
    public ComputePatchRecoveryView()
    {
        this.DataContext = new ComputePatchRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputePatchRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}