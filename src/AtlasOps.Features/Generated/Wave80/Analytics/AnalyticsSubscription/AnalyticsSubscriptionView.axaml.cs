namespace AtlasOps.Features.Analytics.AnalyticsSubscription;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AnalyticsSubscriptionView : UserControl
{
    public AnalyticsSubscriptionView()
    {
        this.DataContext = new AnalyticsSubscriptionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AnalyticsSubscriptionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}