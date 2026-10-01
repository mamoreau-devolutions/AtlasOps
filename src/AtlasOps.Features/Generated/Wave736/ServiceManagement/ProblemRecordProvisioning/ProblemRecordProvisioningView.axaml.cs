namespace AtlasOps.Features.ServiceManagement.ProblemRecordProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ProblemRecordProvisioningView : UserControl
{
    public ProblemRecordProvisioningView()
    {
        this.DataContext = new ProblemRecordProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ProblemRecordProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}