namespace AtlasOps.Features.Database.DatabaseSchemaOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseSchemaOptimizationView : UserControl
{
    public DatabaseSchemaOptimizationView()
    {
        this.DataContext = new DatabaseSchemaOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseSchemaOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}