namespace AtlasOps.Features.Architecture.ArchitectureRiskRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureRiskRecoveryView : UserControl
{
    public ArchitectureRiskRecoveryView()
    {
        this.DataContext = new ArchitectureRiskRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureRiskRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}