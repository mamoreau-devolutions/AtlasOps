namespace AtlasOps.Features.Storage.ObjectBucketProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObjectBucketProvisioningView : UserControl
{
    public ObjectBucketProvisioningView()
    {
        this.DataContext = new ObjectBucketProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObjectBucketProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}