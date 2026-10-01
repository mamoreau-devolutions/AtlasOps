namespace AtlasOps.Features.Architecture.ArchitectureInterfaceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureInterfaceRecoveryView : UserControl
{
    public ArchitectureInterfaceRecoveryView()
    {
        this.DataContext = new ArchitectureInterfaceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureInterfaceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}