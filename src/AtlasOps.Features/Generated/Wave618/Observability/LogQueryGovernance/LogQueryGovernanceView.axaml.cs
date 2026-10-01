namespace AtlasOps.Features.Observability.LogQueryGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LogQueryGovernanceView : UserControl
{
    public LogQueryGovernanceView()
    {
        this.DataContext = new LogQueryGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LogQueryGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}