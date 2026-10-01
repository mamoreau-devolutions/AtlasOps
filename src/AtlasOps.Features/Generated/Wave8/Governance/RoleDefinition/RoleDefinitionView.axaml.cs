namespace AtlasOps.Features.Governance.RoleDefinition;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RoleDefinitionView : UserControl
{
    public RoleDefinitionView()
    {
        this.DataContext = new RoleDefinitionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RoleDefinitionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}