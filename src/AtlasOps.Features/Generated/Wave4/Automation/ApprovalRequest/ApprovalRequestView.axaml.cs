namespace AtlasOps.Features.Automation.ApprovalRequest;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApprovalRequestView : UserControl
{
    public ApprovalRequestView()
    {
        this.DataContext = new ApprovalRequestViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApprovalRequestViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}