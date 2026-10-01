namespace AtlasOps.Features.Governance.PolicyDefinition;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class PolicyDefinitionView : UserControl
{
    public PolicyDefinitionView()
    {
        this.DataContext = new PolicyDefinitionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is PolicyDefinitionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}