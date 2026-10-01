namespace AtlasOps.Features.Hardening.MemoryBudget;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MemoryBudgetView : UserControl
{
    public MemoryBudgetView()
    {
        this.DataContext = new MemoryBudgetViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MemoryBudgetViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}