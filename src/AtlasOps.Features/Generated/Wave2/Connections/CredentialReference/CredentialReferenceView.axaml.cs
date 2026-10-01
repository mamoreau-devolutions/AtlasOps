namespace AtlasOps.Features.Connections.CredentialReference;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CredentialReferenceView : UserControl
{
    public CredentialReferenceView()
    {
        this.DataContext = new CredentialReferenceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CredentialReferenceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}