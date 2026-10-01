namespace AtlasOps.Features.Platform.OperationThrottling;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class OperationThrottlingView : UserControl
{
    public OperationThrottlingView()
    {
        this.DataContext = new OperationThrottlingViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is OperationThrottlingViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}