namespace AtlasOps.Features.Data.DataLineageRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataLineageRecoveryView : UserControl
{
    public DataLineageRecoveryView()
    {
        this.DataContext = new DataLineageRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataLineageRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}