namespace AtlasOps.Features.Architecture.ArchitectureComponentRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureComponentRecoveryView : UserControl
{
    public ArchitectureComponentRecoveryView()
    {
        this.DataContext = new ArchitectureComponentRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureComponentRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}