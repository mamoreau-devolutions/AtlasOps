namespace AtlasOps.Features.FinOps.CloudInvoiceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudInvoiceGovernanceView : UserControl
{
    public CloudInvoiceGovernanceView()
    {
        this.DataContext = new CloudInvoiceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudInvoiceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}