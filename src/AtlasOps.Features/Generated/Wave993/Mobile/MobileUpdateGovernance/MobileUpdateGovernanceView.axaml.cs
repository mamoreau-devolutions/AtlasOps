namespace AtlasOps.Features.Mobile.MobileUpdateGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileUpdateGovernanceView : UserControl
{
    public MobileUpdateGovernanceView()
    {
        this.DataContext = new MobileUpdateGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileUpdateGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}