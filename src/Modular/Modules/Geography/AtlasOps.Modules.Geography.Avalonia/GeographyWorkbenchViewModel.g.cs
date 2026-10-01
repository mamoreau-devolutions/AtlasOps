namespace AtlasOps.Modules.Geography.Avalonia;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using AtlasOps.Modules.Geography.Core;

public sealed class GeographyWorkbenchViewModel : INotifyPropertyChanged
{
    private string _searchText = string.Empty; private GeographyCapabilityDescriptor? _selectedCapability;
    public string Title => GeographyModule.DisplayName;
    public string Summary => string.Concat(GeographyModule.Capabilities.Count, " operational capabilities across five business areas.");
    public IReadOnlyList<GeographyCapabilityDescriptor> Capabilities => string.IsNullOrWhiteSpace(_searchText) ? GeographyModule.Capabilities : GeographyModule.Capabilities.Where(item => item.DisplayName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Area.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Concern.Contains(_searchText, StringComparison.OrdinalIgnoreCase)).ToArray();
    public string SearchText { get => _searchText; set { if (_searchText == value) { return; } _searchText = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged(nameof(Capabilities)); } }
    public GeographyCapabilityDescriptor? SelectedCapability { get => _selectedCapability; set { if (Equals(_selectedCapability, value)) { return; } _selectedCapability = value; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
}