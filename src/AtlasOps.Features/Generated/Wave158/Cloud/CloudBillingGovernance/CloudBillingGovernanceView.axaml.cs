namespace AtlasOps.Features.Cloud.CloudBillingGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudBillingGovernanceView : UserControl
{
    public CloudBillingGovernanceView()
    {
        this.DataContext = new CloudBillingGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudBillingGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}