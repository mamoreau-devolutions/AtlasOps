namespace AtlasOps.Features.Architecture.ArchitectureStandardRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureStandardRecoveryView : UserControl
{
    public ArchitectureStandardRecoveryView()
    {
        this.DataContext = new ArchitectureStandardRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureStandardRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}