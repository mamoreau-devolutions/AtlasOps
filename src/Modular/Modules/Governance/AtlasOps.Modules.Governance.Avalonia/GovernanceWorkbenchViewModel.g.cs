namespace AtlasOps.Modules.Governance.Avalonia;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using AtlasOps.Modules.Governance.Core;

public sealed class GovernanceWorkbenchViewModel : INotifyPropertyChanged
{
    private string _searchText = string.Empty; private GovernanceCapabilityDescriptor? _selectedCapability;
    public string Title => GovernanceModule.DisplayName;
    public string Summary => string.Concat(GovernanceModule.Capabilities.Count, " operational capabilities across five business areas.");
    public IReadOnlyList<GovernanceCapabilityDescriptor> Capabilities => string.IsNullOrWhiteSpace(_searchText) ? GovernanceModule.Capabilities : GovernanceModule.Capabilities.Where(item => item.DisplayName.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Area.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || item.Concern.Contains(_searchText, StringComparison.OrdinalIgnoreCase)).ToArray();
    public string SearchText { get => _searchText; set { if (_searchText == value) { return; } _searchText = value ?? string.Empty; OnPropertyChanged(); OnPropertyChanged(nameof(Capabilities)); } }
    public GovernanceCapabilityDescriptor? SelectedCapability { get => _selectedCapability; set { if (Equals(_selectedCapability, value)) { return; } _selectedCapability = value; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
}