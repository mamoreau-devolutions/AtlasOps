namespace AtlasOps.Features.Governance.RetentionPolicy;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RetentionPolicyView : UserControl
{
    public RetentionPolicyView()
    {
        this.DataContext = new RetentionPolicyViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RetentionPolicyViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}