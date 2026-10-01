namespace AtlasOps.Features.Database.DatabaseRestoreGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseRestoreGovernanceView : UserControl
{
    public DatabaseRestoreGovernanceView()
    {
        this.DataContext = new DatabaseRestoreGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseRestoreGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}