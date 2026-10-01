namespace AtlasOps.Features.Data.DataRetentionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataRetentionProvisioningView : UserControl
{
    public DataRetentionProvisioningView()
    {
        this.DataContext = new DataRetentionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataRetentionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}