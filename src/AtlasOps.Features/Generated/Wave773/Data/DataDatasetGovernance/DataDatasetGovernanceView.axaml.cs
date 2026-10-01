namespace AtlasOps.Features.Data.DataDatasetGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataDatasetGovernanceView : UserControl
{
    public DataDatasetGovernanceView()
    {
        this.DataContext = new DataDatasetGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataDatasetGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}