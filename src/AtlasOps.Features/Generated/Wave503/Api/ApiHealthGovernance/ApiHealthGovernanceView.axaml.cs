namespace AtlasOps.Features.Api.ApiHealthGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiHealthGovernanceView : UserControl
{
    public ApiHealthGovernanceView()
    {
        this.DataContext = new ApiHealthGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiHealthGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}