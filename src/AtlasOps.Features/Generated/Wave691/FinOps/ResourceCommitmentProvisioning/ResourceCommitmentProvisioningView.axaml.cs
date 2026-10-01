namespace AtlasOps.Features.FinOps.ResourceCommitmentProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ResourceCommitmentProvisioningView : UserControl
{
    public ResourceCommitmentProvisioningView()
    {
        this.DataContext = new ResourceCommitmentProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ResourceCommitmentProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}