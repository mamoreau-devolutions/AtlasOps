namespace AtlasOps.Features.Compute.ComputeLifecycleRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeLifecycleRecoveryView : UserControl
{
    public ComputeLifecycleRecoveryView()
    {
        this.DataContext = new ComputeLifecycleRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeLifecycleRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}