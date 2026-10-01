namespace AtlasOps.Features.Governance.ResourceScope;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ResourceScopeView : UserControl
{
    public ResourceScopeView()
    {
        this.DataContext = new ResourceScopeViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ResourceScopeViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}