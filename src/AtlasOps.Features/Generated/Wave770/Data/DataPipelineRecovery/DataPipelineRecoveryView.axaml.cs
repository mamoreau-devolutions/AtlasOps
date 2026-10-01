namespace AtlasOps.Features.Data.DataPipelineRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataPipelineRecoveryView : UserControl
{
    public DataPipelineRecoveryView()
    {
        this.DataContext = new DataPipelineRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataPipelineRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}