namespace AtlasOps.Features.Database.DatabaseRestoreProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseRestoreProvisioningView : UserControl
{
    public DatabaseRestoreProvisioningView()
    {
        this.DataContext = new DatabaseRestoreProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseRestoreProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}