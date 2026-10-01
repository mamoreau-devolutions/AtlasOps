namespace AtlasOps.Features.Cloud.AzureSubscriptionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AzureSubscriptionGovernanceView : UserControl
{
    public AzureSubscriptionGovernanceView()
    {
        this.DataContext = new AzureSubscriptionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AzureSubscriptionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}