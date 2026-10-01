namespace AtlasOps.Features.Data.DataProductRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataProductRecoveryView : UserControl
{
    public DataProductRecoveryView()
    {
        this.DataContext = new DataProductRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataProductRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}