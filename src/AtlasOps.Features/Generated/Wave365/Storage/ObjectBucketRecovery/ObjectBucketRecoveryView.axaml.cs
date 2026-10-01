namespace AtlasOps.Features.Storage.ObjectBucketRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObjectBucketRecoveryView : UserControl
{
    public ObjectBucketRecoveryView()
    {
        this.DataContext = new ObjectBucketRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObjectBucketRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}