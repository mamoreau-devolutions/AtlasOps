namespace AtlasOps.Features.Architecture.ArchitectureReviewProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureReviewProvisioningView : UserControl
{
    public ArchitectureReviewProvisioningView()
    {
        this.DataContext = new ArchitectureReviewProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureReviewProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}