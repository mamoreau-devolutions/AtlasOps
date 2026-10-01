namespace AtlasOps.Features.Storage.FileShareGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FileShareGovernanceView : UserControl
{
    public FileShareGovernanceView()
    {
        this.DataContext = new FileShareGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FileShareGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}