namespace AtlasOps.Features.Api.ApiHealthRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiHealthRecoveryView : UserControl
{
    public ApiHealthRecoveryView()
    {
        this.DataContext = new ApiHealthRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiHealthRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}