namespace AtlasOps.Features.Sync.ChangeVector;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ChangeVectorView : UserControl
{
    public ChangeVectorView()
    {
        this.DataContext = new ChangeVectorViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ChangeVectorViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}