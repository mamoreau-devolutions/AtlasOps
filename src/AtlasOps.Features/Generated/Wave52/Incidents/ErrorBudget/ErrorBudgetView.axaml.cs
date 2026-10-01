namespace AtlasOps.Features.Incidents.ErrorBudget;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ErrorBudgetView : UserControl
{
    public ErrorBudgetView()
    {
        this.DataContext = new ErrorBudgetViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ErrorBudgetViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}