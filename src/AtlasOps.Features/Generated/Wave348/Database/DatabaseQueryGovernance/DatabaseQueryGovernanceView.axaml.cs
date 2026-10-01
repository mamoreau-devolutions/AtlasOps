namespace AtlasOps.Features.Database.DatabaseQueryGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseQueryGovernanceView : UserControl
{
    public DatabaseQueryGovernanceView()
    {
        this.DataContext = new DatabaseQueryGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseQueryGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}