namespace AtlasOps.Features.Database.DatabaseQueryOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseQueryOptimizationView : UserControl
{
    public DatabaseQueryOptimizationView()
    {
        this.DataContext = new DatabaseQueryOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseQueryOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}