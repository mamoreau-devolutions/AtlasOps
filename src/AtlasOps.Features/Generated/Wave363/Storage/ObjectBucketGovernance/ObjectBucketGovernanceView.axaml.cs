namespace AtlasOps.Features.Storage.ObjectBucketGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObjectBucketGovernanceView : UserControl
{
    public ObjectBucketGovernanceView()
    {
        this.DataContext = new ObjectBucketGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObjectBucketGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}