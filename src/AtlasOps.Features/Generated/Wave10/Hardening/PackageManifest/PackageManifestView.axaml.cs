namespace AtlasOps.Features.Hardening.PackageManifest;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class PackageManifestView : UserControl
{
    public PackageManifestView()
    {
        this.DataContext = new PackageManifestViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is PackageManifestViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}