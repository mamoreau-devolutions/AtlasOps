namespace AtlasOps.Features.FinOps.FinOpsReportRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FinOpsReportRecoveryView : UserControl
{
    public FinOpsReportRecoveryView()
    {
        this.DataContext = new FinOpsReportRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FinOpsReportRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}