namespace AtlasOps.Features.Architecture.ArchitectureEvidenceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureEvidenceRecoveryView : UserControl
{
    public ArchitectureEvidenceRecoveryView()
    {
        this.DataContext = new ArchitectureEvidenceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureEvidenceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}