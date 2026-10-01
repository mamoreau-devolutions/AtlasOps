namespace AtlasOps.Features.Hardening.PerformanceBudget;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class PerformanceBudgetView : UserControl
{
    public PerformanceBudgetView()
    {
        this.DataContext = new PerformanceBudgetViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is PerformanceBudgetViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}