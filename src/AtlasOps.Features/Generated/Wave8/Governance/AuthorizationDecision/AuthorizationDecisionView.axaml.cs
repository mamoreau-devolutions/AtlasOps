namespace AtlasOps.Features.Governance.AuthorizationDecision;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AuthorizationDecisionView : UserControl
{
    public AuthorizationDecisionView()
    {
        this.DataContext = new AuthorizationDecisionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AuthorizationDecisionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}