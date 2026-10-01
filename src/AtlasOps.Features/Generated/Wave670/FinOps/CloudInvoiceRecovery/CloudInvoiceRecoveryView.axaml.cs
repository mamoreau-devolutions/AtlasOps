namespace AtlasOps.Features.FinOps.CloudInvoiceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudInvoiceRecoveryView : UserControl
{
    public CloudInvoiceRecoveryView()
    {
        this.DataContext = new CloudInvoiceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudInvoiceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}