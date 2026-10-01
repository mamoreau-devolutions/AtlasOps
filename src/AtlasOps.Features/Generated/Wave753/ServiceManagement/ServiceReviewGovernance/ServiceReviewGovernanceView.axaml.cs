namespace AtlasOps.Features.ServiceManagement.ServiceReviewGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceReviewGovernanceView : UserControl
{
    public ServiceReviewGovernanceView()
    {
        this.DataContext = new ServiceReviewGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceReviewGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}