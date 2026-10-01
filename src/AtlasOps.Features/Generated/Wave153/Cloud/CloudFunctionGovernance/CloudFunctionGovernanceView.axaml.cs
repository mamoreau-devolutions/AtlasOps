namespace AtlasOps.Features.Cloud.CloudFunctionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudFunctionGovernanceView : UserControl
{
    public CloudFunctionGovernanceView()
    {
        this.DataContext = new CloudFunctionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudFunctionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}