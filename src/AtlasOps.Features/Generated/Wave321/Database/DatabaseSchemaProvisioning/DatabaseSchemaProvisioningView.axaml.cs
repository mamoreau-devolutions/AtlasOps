namespace AtlasOps.Features.Database.DatabaseSchemaProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseSchemaProvisioningView : UserControl
{
    public DatabaseSchemaProvisioningView()
    {
        this.DataContext = new DatabaseSchemaProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseSchemaProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}