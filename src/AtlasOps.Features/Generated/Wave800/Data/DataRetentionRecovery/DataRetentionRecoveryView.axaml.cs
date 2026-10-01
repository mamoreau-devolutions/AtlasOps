namespace AtlasOps.Features.Data.DataRetentionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataRetentionRecoveryView : UserControl
{
    public DataRetentionRecoveryView()
    {
        this.DataContext = new DataRetentionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataRetentionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}