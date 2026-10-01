namespace AtlasOps.Features.Mobile.MobilePolicyGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobilePolicyGovernanceView : UserControl
{
    public MobilePolicyGovernanceView()
    {
        this.DataContext = new MobilePolicyGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobilePolicyGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}