namespace AtlasOps.Features.Cloud.CloudIdentityGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudIdentityGovernanceView : UserControl
{
    public CloudIdentityGovernanceView()
    {
        this.DataContext = new CloudIdentityGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudIdentityGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}