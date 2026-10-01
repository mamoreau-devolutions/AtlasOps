namespace AtlasOps.Features.ServiceManagement.ServiceReviewRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceReviewRecoveryView : UserControl
{
    public ServiceReviewRecoveryView()
    {
        this.DataContext = new ServiceReviewRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceReviewRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}