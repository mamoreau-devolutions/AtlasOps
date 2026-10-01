namespace AtlasOps.Features.Data.DataProductGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataProductGovernanceView : UserControl
{
    public DataProductGovernanceView()
    {
        this.DataContext = new DataProductGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataProductGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}