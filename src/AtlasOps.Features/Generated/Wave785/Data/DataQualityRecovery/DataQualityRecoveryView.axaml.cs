namespace AtlasOps.Features.Data.DataQualityRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataQualityRecoveryView : UserControl
{
    public DataQualityRecoveryView()
    {
        this.DataContext = new DataQualityRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataQualityRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}