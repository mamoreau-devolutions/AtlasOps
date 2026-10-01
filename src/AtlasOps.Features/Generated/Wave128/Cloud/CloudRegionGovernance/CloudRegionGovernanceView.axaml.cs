namespace AtlasOps.Features.Cloud.CloudRegionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudRegionGovernanceView : UserControl
{
    public CloudRegionGovernanceView()
    {
        this.DataContext = new CloudRegionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudRegionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}