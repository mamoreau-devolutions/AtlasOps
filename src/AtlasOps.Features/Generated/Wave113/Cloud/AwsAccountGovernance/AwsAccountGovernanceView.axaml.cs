namespace AtlasOps.Features.Cloud.AwsAccountGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AwsAccountGovernanceView : UserControl
{
    public AwsAccountGovernanceView()
    {
        this.DataContext = new AwsAccountGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AwsAccountGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}