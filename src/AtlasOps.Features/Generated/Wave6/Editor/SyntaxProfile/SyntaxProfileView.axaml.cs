namespace AtlasOps.Features.Editor.SyntaxProfile;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SyntaxProfileView : UserControl
{
    public SyntaxProfileView()
    {
        this.DataContext = new SyntaxProfileViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SyntaxProfileViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}