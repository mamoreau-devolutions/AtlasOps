namespace AtlasOps.Features.Delivery.ReleasePipelineRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleasePipelineRecoveryView : UserControl
{
    public ReleasePipelineRecoveryView()
    {
        this.DataContext = new ReleasePipelineRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleasePipelineRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}