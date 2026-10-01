namespace AtlasOps.Features.Data.DataRetentionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataRetentionGovernanceView : UserControl
{
    public DataRetentionGovernanceView()
    {
        this.DataContext = new DataRetentionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataRetentionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}