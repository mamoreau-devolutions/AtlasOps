namespace AtlasOps.Features.Storage.FileShareProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FileShareProvisioningView : UserControl
{
    public FileShareProvisioningView()
    {
        this.DataContext = new FileShareProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FileShareProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}