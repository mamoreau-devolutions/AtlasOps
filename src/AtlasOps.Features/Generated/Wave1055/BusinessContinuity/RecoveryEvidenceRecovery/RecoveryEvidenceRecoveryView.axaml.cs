namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoveryEvidenceRecoveryView : UserControl
{
    public RecoveryEvidenceRecoveryView()
    {
        this.DataContext = new RecoveryEvidenceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoveryEvidenceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}