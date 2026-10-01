namespace AtlasOps.Features.Database.DatabaseCredentialGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseCredentialGovernanceView : UserControl
{
    public DatabaseCredentialGovernanceView()
    {
        this.DataContext = new DatabaseCredentialGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseCredentialGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}