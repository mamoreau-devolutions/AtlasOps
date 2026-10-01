namespace AtlasOps.Features.Api.ApiTokenRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiTokenRecoveryView : UserControl
{
    public ApiTokenRecoveryView()
    {
        this.DataContext = new ApiTokenRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiTokenRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}