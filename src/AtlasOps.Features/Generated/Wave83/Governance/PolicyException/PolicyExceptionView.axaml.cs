namespace AtlasOps.Features.Governance.PolicyException;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class PolicyExceptionView : UserControl
{
    public PolicyExceptionView()
    {
        this.DataContext = new PolicyExceptionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is PolicyExceptionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}