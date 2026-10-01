namespace AtlasOps.Features.Data.DataSourceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataSourceGovernanceView : UserControl
{
    public DataSourceGovernanceView()
    {
        this.DataContext = new DataSourceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataSourceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}