namespace AtlasOps.Features.Compute.ComputeConsoleRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeConsoleRecoveryView : UserControl
{
    public ComputeConsoleRecoveryView()
    {
        this.DataContext = new ComputeConsoleRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeConsoleRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}