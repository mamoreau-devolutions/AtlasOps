namespace AtlasOps.Features.Architecture.ArchitectureDecisionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureDecisionRecoveryView : UserControl
{
    public ArchitectureDecisionRecoveryView()
    {
        this.DataContext = new ArchitectureDecisionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureDecisionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}