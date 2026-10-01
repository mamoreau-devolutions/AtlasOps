namespace AtlasOps.Features.Sync.RestoreOperation;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RestoreOperationView : UserControl
{
    public RestoreOperationView()
    {
        this.DataContext = new RestoreOperationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RestoreOperationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}