namespace AtlasOps.Features.Data.DataTransformRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataTransformRecoveryView : UserControl
{
    public DataTransformRecoveryView()
    {
        this.DataContext = new DataTransformRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataTransformRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}