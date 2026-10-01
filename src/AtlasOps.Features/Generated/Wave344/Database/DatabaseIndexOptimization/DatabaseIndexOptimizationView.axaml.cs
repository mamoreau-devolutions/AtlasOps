namespace AtlasOps.Features.Database.DatabaseIndexOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseIndexOptimizationView : UserControl
{
    public DatabaseIndexOptimizationView()
    {
        this.DataContext = new DatabaseIndexOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseIndexOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}