namespace AtlasOps.Features.Architecture.ArchitectureDependencyProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureDependencyProvisioningView : UserControl
{
    public ArchitectureDependencyProvisioningView()
    {
        this.DataContext = new ArchitectureDependencyProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureDependencyProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}