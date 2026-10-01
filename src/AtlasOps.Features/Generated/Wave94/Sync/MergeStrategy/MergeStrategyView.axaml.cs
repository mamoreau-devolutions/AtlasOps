namespace AtlasOps.Features.Sync.MergeStrategy;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MergeStrategyView : UserControl
{
    public MergeStrategyView()
    {
        this.DataContext = new MergeStrategyViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MergeStrategyViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}