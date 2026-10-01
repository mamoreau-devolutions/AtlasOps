namespace AtlasOps.Features.Data.DataAccessGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataAccessGovernanceView : UserControl
{
    public DataAccessGovernanceView()
    {
        this.DataContext = new DataAccessGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataAccessGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}