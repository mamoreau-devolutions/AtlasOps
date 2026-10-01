namespace AtlasOps.Features.Inventory.DependencyMapping;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DependencyMappingView : UserControl
{
    public DependencyMappingView()
    {
        this.DataContext = new DependencyMappingViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DependencyMappingViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}