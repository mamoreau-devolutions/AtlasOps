namespace AtlasOps.Features.Architecture.ArchitectureEvidenceProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureEvidenceProvisioningView : UserControl
{
    public ArchitectureEvidenceProvisioningView()
    {
        this.DataContext = new ArchitectureEvidenceProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureEvidenceProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}