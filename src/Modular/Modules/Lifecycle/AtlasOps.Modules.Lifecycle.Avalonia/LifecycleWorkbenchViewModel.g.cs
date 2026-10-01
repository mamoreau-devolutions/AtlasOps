namespace AtlasOps.Modules.Lifecycle.Avalonia;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using AtlasOps.Modules.Lifecycle.Core;

public sealed class LifecycleWorkbenchViewModel : INotifyPropertyChanged
{
    private string _searchText = string.Empty; private LifecycleCapabilityDescriptor? _selectedCapability;
    public string Title => LifecycleModule.DisplayName;
    public string Summary => string.Concat(LifecycleModule.Capabilities.Count, " operational capabilities across five business areas.");
    public IReadOnlyList<LifecycleCapabilityDescriptor> Capabilities => string.IsNullOrWhiteSpace(_searchText) ? LifecycleModule.Capabilities : LifecycleModule.Capabilities.Where(item => item.DisplayName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Area.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Concern.Contains(_searchText, StringComparison.OrdinalIgnoreCase)).ToArray();
    public string SearchText { get => _searchText; set { if (_searchText == value) { return; } _searchText = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged(nameof(Capabilities)); } }
    public LifecycleCapabilityDescriptor? SelectedCapability { get => _selectedCapability; set { if (Equals(_selectedCapability, value)) { return; } _selectedCapability = value; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
}