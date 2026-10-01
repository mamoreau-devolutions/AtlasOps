namespace AtlasOps.Features.BusinessContinuity.RecoverySiteGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RecoverySiteGovernanceView : UserControl
{
    public RecoverySiteGovernanceView()
    {
        this.DataContext = new RecoverySiteGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RecoverySiteGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}