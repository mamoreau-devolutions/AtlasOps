namespace AtlasOps.Features.Connections.HostKeyVerification;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class HostKeyVerificationView : UserControl
{
    public HostKeyVerificationView()
    {
        this.DataContext = new HostKeyVerificationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is HostKeyVerificationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}