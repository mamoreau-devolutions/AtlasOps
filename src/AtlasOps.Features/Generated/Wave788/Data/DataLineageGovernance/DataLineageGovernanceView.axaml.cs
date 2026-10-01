namespace AtlasOps.Features.Data.DataLineageGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataLineageGovernanceView : UserControl
{
    public DataLineageGovernanceView()
    {
        this.DataContext = new DataLineageGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataLineageGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}