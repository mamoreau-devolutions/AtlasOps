namespace AtlasOps.Features.Architecture.ArchitectureStandardProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureStandardProvisioningView : UserControl
{
    public ArchitectureStandardProvisioningView()
    {
        this.DataContext = new ArchitectureStandardProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureStandardProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}