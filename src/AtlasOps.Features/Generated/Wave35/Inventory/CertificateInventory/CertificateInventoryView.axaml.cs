namespace AtlasOps.Features.Inventory.CertificateInventory;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CertificateInventoryView : UserControl
{
    public CertificateInventoryView()
    {
        this.DataContext = new CertificateInventoryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CertificateInventoryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}