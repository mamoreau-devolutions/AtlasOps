namespace AtlasOps.Features.Cloud.CloudDatabaseRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudDatabaseRecoveryView : UserControl
{
    public CloudDatabaseRecoveryView()
    {
        this.DataContext = new CloudDatabaseRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudDatabaseRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}