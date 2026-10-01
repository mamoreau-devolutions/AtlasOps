namespace AtlasOps.Features.Database.DatabaseRestoreOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseRestoreOptimizationView : UserControl
{
    public DatabaseRestoreOptimizationView()
    {
        this.DataContext = new DatabaseRestoreOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseRestoreOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}