namespace AtlasOps.Features.Architecture.ArchitectureExceptionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureExceptionRecoveryView : UserControl
{
    public ArchitectureExceptionRecoveryView()
    {
        this.DataContext = new ArchitectureExceptionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureExceptionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}