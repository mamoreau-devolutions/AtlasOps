namespace AtlasOps.Features.Cloud.CloudDatabaseGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudDatabaseGovernanceView : UserControl
{
    public CloudDatabaseGovernanceView()
    {
        this.DataContext = new CloudDatabaseGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudDatabaseGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}