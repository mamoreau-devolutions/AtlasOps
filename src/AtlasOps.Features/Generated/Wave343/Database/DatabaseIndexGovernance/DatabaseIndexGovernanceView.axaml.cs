namespace AtlasOps.Features.Database.DatabaseIndexGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseIndexGovernanceView : UserControl
{
    public DatabaseIndexGovernanceView()
    {
        this.DataContext = new DatabaseIndexGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseIndexGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}