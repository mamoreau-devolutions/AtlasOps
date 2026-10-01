namespace AtlasOps.Features.Connections.SecretLease;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecretLeaseView : UserControl
{
    public SecretLeaseView()
    {
        this.DataContext = new SecretLeaseViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecretLeaseViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}