namespace AtlasOps.Features.Analytics.SharedFilter;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SharedFilterView : UserControl
{
    public SharedFilterView()
    {
        this.DataContext = new SharedFilterViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SharedFilterViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}