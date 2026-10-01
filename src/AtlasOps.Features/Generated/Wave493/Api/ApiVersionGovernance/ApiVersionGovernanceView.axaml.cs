namespace AtlasOps.Features.Api.ApiVersionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiVersionGovernanceView : UserControl
{
    public ApiVersionGovernanceView()
    {
        this.DataContext = new ApiVersionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiVersionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}