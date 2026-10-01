namespace AtlasOps.Features.Hardening.AccessibilityProfile;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AccessibilityProfileView : UserControl
{
    public AccessibilityProfileView()
    {
        this.DataContext = new AccessibilityProfileViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AccessibilityProfileViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}