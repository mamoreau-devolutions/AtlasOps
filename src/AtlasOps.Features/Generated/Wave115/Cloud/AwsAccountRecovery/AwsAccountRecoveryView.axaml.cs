namespace AtlasOps.Features.Cloud.AwsAccountRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AwsAccountRecoveryView : UserControl
{
    public AwsAccountRecoveryView()
    {
        this.DataContext = new AwsAccountRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AwsAccountRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}