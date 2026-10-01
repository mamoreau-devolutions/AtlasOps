namespace AtlasOps.Features.Governance.OwnershipRule;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class OwnershipRuleView : UserControl
{
    public OwnershipRuleView()
    {
        this.DataContext = new OwnershipRuleViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is OwnershipRuleViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}