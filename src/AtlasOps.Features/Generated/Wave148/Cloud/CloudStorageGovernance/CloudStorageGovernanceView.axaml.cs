namespace AtlasOps.Features.Cloud.CloudStorageGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudStorageGovernanceView : UserControl
{
    public CloudStorageGovernanceView()
    {
        this.DataContext = new CloudStorageGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudStorageGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}