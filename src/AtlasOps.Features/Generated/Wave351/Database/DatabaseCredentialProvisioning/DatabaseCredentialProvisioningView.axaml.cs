namespace AtlasOps.Features.Database.DatabaseCredentialProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseCredentialProvisioningView : UserControl
{
    public DatabaseCredentialProvisioningView()
    {
        this.DataContext = new DatabaseCredentialProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseCredentialProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}