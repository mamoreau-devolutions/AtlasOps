namespace AtlasOps.Features.ServiceManagement.ChangeRequestRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ChangeRequestRecoveryView : UserControl
{
    public ChangeRequestRecoveryView()
    {
        this.DataContext = new ChangeRequestRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ChangeRequestRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}