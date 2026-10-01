namespace AtlasOps.Features.Data.DataQualityGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataQualityGovernanceView : UserControl
{
    public DataQualityGovernanceView()
    {
        this.DataContext = new DataQualityGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataQualityGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}