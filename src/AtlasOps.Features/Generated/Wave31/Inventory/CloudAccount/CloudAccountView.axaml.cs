namespace AtlasOps.Features.Inventory.CloudAccount;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudAccountView : UserControl
{
    public CloudAccountView()
    {
        this.DataContext = new CloudAccountViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudAccountViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}