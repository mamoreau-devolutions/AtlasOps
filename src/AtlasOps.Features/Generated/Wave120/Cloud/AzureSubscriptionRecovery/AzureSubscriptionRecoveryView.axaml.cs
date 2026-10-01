namespace AtlasOps.Features.Cloud.AzureSubscriptionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AzureSubscriptionRecoveryView : UserControl
{
    public AzureSubscriptionRecoveryView()
    {
        this.DataContext = new AzureSubscriptionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AzureSubscriptionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}