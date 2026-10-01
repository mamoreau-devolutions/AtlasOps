namespace AtlasOps.Features.Api.ApiTokenGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiTokenGovernanceView : UserControl
{
    public ApiTokenGovernanceView()
    {
        this.DataContext = new ApiTokenGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiTokenGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}