namespace AtlasOps.Features.ServiceManagement.ChangeRequestGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ChangeRequestGovernanceView : UserControl
{
    public ChangeRequestGovernanceView()
    {
        this.DataContext = new ChangeRequestGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ChangeRequestGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}