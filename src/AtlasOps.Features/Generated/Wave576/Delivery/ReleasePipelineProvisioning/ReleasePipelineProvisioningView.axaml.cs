namespace AtlasOps.Features.Delivery.ReleasePipelineProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleasePipelineProvisioningView : UserControl
{
    public ReleasePipelineProvisioningView()
    {
        this.DataContext = new ReleasePipelineProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleasePipelineProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}