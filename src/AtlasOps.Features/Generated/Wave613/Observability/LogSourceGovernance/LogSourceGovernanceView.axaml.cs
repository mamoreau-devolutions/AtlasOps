namespace AtlasOps.Features.Observability.LogSourceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LogSourceGovernanceView : UserControl
{
    public LogSourceGovernanceView()
    {
        this.DataContext = new LogSourceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LogSourceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}