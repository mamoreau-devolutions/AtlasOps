namespace AtlasOps.Features.Database.DatabaseReplicaRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseReplicaRecoveryView : UserControl
{
    public DatabaseReplicaRecoveryView()
    {
        this.DataContext = new DatabaseReplicaRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseReplicaRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}