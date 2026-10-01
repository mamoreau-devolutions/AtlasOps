namespace AtlasOps.Features.FinOps.SavingsPlanGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SavingsPlanGovernanceView : UserControl
{
    public SavingsPlanGovernanceView()
    {
        this.DataContext = new SavingsPlanGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SavingsPlanGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}