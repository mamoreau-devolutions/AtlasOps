namespace AtlasOps.Features.FinOps.SpendForecastGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SpendForecastGovernanceView : UserControl
{
    public SpendForecastGovernanceView()
    {
        this.DataContext = new SpendForecastGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SpendForecastGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}