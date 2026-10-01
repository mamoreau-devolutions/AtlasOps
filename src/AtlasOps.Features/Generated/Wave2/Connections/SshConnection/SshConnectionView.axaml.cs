namespace AtlasOps.Features.Connections.SshConnection;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SshConnectionView : UserControl
{
    public SshConnectionView()
    {
        this.DataContext = new SshConnectionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SshConnectionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}