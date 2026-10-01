namespace AtlasOps.Features.Data.DataSourceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataSourceRecoveryView : UserControl
{
    public DataSourceRecoveryView()
    {
        this.DataContext = new DataSourceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataSourceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}