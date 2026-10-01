namespace AtlasOps.Features.Connections.TunnelDefinition;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TunnelDefinitionView : UserControl
{
    public TunnelDefinitionView()
    {
        this.DataContext = new TunnelDefinitionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TunnelDefinitionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}