namespace AtlasOps.Features.Api.ApiQuotaRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiQuotaRecoveryView : UserControl
{
    public ApiQuotaRecoveryView()
    {
        this.DataContext = new ApiQuotaRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiQuotaRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}