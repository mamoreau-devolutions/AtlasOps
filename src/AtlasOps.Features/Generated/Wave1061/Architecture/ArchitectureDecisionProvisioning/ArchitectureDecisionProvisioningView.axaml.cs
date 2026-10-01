namespace AtlasOps.Features.Architecture.ArchitectureDecisionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureDecisionProvisioningView : UserControl
{
    public ArchitectureDecisionProvisioningView()
    {
        this.DataContext = new ArchitectureDecisionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureDecisionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}