namespace AtlasOps.Features.Governance.ActorIdentity;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ActorIdentityView : UserControl
{
    public ActorIdentityView()
    {
        this.DataContext = new ActorIdentityViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ActorIdentityViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}