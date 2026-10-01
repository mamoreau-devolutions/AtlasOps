namespace AtlasOps.Features.Compute.ComputeImageRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeImageRecoveryView : UserControl
{
    public ComputeImageRecoveryView()
    {
        this.DataContext = new ComputeImageRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeImageRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}