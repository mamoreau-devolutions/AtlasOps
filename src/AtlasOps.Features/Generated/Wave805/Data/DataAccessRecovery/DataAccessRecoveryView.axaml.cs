namespace AtlasOps.Features.Data.DataAccessRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataAccessRecoveryView : UserControl
{
    public DataAccessRecoveryView()
    {
        this.DataContext = new DataAccessRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataAccessRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}