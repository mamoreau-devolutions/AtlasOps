namespace AtlasOps.Modules.Localization.Avalonia;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using AtlasOps.Modules.Localization.Core;

public sealed class LocalizationWorkbenchViewModel : INotifyPropertyChanged
{
    private string _searchText = string.Empty; private LocalizationCapabilityDescriptor? _selectedCapability;
    public string Title => LocalizationModule.DisplayName;
    public string Summary => string.Concat(LocalizationModule.Capabilities.Count, " operational capabilities across five business areas.");
    public IReadOnlyList<LocalizationCapabilityDescriptor> Capabilities => string.IsNullOrWhiteSpace(_searchText) ? LocalizationModule.Capabilities : LocalizationModule.Capabilities.Where(item => item.DisplayName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Area.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Concern.Contains(_searchText, StringComparison.OrdinalIgnoreCase)).ToArray();
    public string SearchText { get => _searchText; set { if (_searchText == value) { return; } _searchText = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged(nameof(Capabilities)); } }
    public LocalizationCapabilityDescriptor? SelectedCapability { get => _selectedCapability; set { if (Equals(_selectedCapability, value)) { return; } _selectedCapability = value; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
}