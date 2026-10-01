namespace AtlasOps.Features.Incidents.StakeholderSubscription;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StakeholderSubscriptionView : UserControl
{
    public StakeholderSubscriptionView()
    {
        this.DataContext = new StakeholderSubscriptionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StakeholderSubscriptionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}