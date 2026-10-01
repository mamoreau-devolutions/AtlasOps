namespace AtlasOps.Features.Edge.EdgeDeviceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeDeviceGovernanceView : UserControl
{
    public EdgeDeviceGovernanceView()
    {
        this.DataContext = new EdgeDeviceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeDeviceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}