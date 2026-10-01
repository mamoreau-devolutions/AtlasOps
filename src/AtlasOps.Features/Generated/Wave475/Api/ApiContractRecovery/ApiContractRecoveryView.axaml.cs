namespace AtlasOps.Features.Api.ApiContractRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiContractRecoveryView : UserControl
{
    public ApiContractRecoveryView()
    {
        this.DataContext = new ApiContractRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiContractRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}