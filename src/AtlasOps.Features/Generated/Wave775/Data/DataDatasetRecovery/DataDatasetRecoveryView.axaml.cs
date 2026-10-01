namespace AtlasOps.Features.Data.DataDatasetRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataDatasetRecoveryView : UserControl
{
    public DataDatasetRecoveryView()
    {
        this.DataContext = new DataDatasetRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataDatasetRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}