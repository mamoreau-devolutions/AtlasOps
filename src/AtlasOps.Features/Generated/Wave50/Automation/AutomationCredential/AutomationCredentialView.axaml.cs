namespace AtlasOps.Features.Automation.AutomationCredential;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AutomationCredentialView : UserControl
{
    public AutomationCredentialView()
    {
        this.DataContext = new AutomationCredentialViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AutomationCredentialViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}