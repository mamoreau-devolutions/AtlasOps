namespace AtlasOps.Features.Automation.RunbookDefinition;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RunbookDefinitionView : UserControl
{
    public RunbookDefinitionView()
    {
        this.DataContext = new RunbookDefinitionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RunbookDefinitionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}