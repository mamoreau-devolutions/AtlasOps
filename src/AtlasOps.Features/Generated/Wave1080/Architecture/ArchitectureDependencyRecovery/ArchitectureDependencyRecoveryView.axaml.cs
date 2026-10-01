namespace AtlasOps.Features.Architecture.ArchitectureDependencyRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureDependencyRecoveryView : UserControl
{
    public ArchitectureDependencyRecoveryView()
    {
        this.DataContext = new ArchitectureDependencyRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureDependencyRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}