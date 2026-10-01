namespace AtlasOps.Features.Database.DatabaseSchemaGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseSchemaGovernanceView : UserControl
{
    public DatabaseSchemaGovernanceView()
    {
        this.DataContext = new DatabaseSchemaGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseSchemaGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}