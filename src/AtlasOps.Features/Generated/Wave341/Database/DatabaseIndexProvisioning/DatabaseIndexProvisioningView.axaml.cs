namespace AtlasOps.Features.Database.DatabaseIndexProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseIndexProvisioningView : UserControl
{
    public DatabaseIndexProvisioningView()
    {
        this.DataContext = new DatabaseIndexProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseIndexProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}