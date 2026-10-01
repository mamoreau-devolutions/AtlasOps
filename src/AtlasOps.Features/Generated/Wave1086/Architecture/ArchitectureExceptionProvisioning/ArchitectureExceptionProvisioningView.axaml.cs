namespace AtlasOps.Features.Architecture.ArchitectureExceptionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureExceptionProvisioningView : UserControl
{
    public ArchitectureExceptionProvisioningView()
    {
        this.DataContext = new ArchitectureExceptionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureExceptionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}