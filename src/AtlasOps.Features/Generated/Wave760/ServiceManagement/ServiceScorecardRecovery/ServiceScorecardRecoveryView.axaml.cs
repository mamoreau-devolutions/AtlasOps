namespace AtlasOps.Features.ServiceManagement.ServiceScorecardRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceScorecardRecoveryView : UserControl
{
    public ServiceScorecardRecoveryView()
    {
        this.DataContext = new ServiceScorecardRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceScorecardRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}