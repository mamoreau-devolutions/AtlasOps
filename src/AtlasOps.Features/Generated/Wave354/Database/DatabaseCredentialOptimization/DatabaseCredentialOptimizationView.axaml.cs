namespace AtlasOps.Features.Database.DatabaseCredentialOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseCredentialOptimizationView : UserControl
{
    public DatabaseCredentialOptimizationView()
    {
        this.DataContext = new DatabaseCredentialOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseCredentialOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}