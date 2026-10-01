namespace AtlasOps.Modules.Identity.Avalonia;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using AtlasOps.Modules.Identity.Core;

public sealed class IdentityWorkbenchViewModel : INotifyPropertyChanged
{
    private string _searchText = string.Empty; private IdentityCapabilityDescriptor? _selectedCapability;
    public string Title => IdentityModule.DisplayName;
    public string Summary => string.Concat(IdentityModule.Capabilities.Count, " operational capabilities across five business areas.");
    public IReadOnlyList<IdentityCapabilityDescriptor> Capabilities => string.IsNullOrWhiteSpace(_searchText) ? IdentityModule.Capabilities : IdentityModule.Capabilities.Where(item => item.DisplayName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Area.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Concern.Contains(_searchText, StringComparison.OrdinalIgnoreCase)).ToArray();
    public string SearchText { get => _searchText; set { if (_searchText == value) { return; } _searchText = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged(nameof(Capabilities)); } }
    public IdentityCapabilityDescriptor? SelectedCapability { get => _selectedCapability; set { if (Equals(_selectedCapability, value)) { return; } _selectedCapability = value; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
}