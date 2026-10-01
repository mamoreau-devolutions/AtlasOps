namespace AtlasOps.Features.Api.ApiAnalyticsGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiAnalyticsGovernanceView : UserControl
{
    public ApiAnalyticsGovernanceView()
    {
        this.DataContext = new ApiAnalyticsGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiAnalyticsGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}