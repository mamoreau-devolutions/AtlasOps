namespace AtlasOps.Features.Database.DatabaseReplicaProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseReplicaProvisioningView : UserControl
{
    public DatabaseReplicaProvisioningView()
    {
        this.DataContext = new DatabaseReplicaProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseReplicaProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}