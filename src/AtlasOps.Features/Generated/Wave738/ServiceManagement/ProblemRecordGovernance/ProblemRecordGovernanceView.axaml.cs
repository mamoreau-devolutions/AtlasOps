namespace AtlasOps.Features.ServiceManagement.ProblemRecordGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ProblemRecordGovernanceView : UserControl
{
    public ProblemRecordGovernanceView()
    {
        this.DataContext = new ProblemRecordGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ProblemRecordGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}