namespace AtlasOps.Modules.Automation.Avalonia;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using AtlasOps.Modules.Automation.Core;

public sealed class AutomationWorkbenchViewModel : INotifyPropertyChanged
{
    private string _searchText = string.Empty; private AutomationCapabilityDescriptor? _selectedCapability;
    public string Title => AutomationModule.DisplayName;
    public string Summary => string.Concat(AutomationModule.Capabilities.Count, " operational capabilities across five business areas.");
    public IReadOnlyList<AutomationCapabilityDescriptor> Capabilities => string.IsNullOrWhiteSpace(_searchText) ? AutomationModule.Capabilities : AutomationModule.Capabilities.Where(item => item.DisplayName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Area.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Concern.Contains(_searchText, StringComparison.OrdinalIgnoreCase)).ToArray();
    public string SearchText { get => _searchText; set { if (_searchText == value) { return; } _searchText = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged(nameof(Capabilities)); } }
    public AutomationCapabilityDescriptor? SelectedCapability { get => _selectedCapability; set { if (Equals(_selectedCapability, value)) { return; } _selectedCapability = value; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
}