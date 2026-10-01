namespace AtlasOps.Features.Platform.BackgroundOperation;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BackgroundOperationView : UserControl
{
    public BackgroundOperationView()
    {
        this.DataContext = new BackgroundOperationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BackgroundOperationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}