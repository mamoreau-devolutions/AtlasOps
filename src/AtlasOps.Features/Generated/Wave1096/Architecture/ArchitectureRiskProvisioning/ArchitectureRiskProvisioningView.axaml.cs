namespace AtlasOps.Features.Architecture.ArchitectureRiskProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureRiskProvisioningView : UserControl
{
    public ArchitectureRiskProvisioningView()
    {
        this.DataContext = new ArchitectureRiskProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureRiskProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}