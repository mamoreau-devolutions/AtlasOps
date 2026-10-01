namespace AtlasOps.Features.Database.DatabaseQueryProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseQueryProvisioningView : UserControl
{
    public DatabaseQueryProvisioningView()
    {
        this.DataContext = new DatabaseQueryProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseQueryProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}