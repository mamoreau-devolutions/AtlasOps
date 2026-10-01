namespace AtlasOps.Modules.Localization.Core;

using System.Collections.Generic;

public sealed record LocalizationCapabilityDescriptor(string Id, string DisplayName, string Area, string Concern, string Description);
public static class LocalizationModule
{
    public const string Id = "Localization";
    public const string DisplayName = "Localization and time zones";
    public static IReadOnlyList<LocalizationCapabilityDescriptor> Capabilities { get; } = new LocalizationCapabilityDescriptor[]
    {
        new("Localization.LocaleCatalog", "Locale catalog", "Locale", "Catalog", "Coordinates localization and time zones for Locale catalog."),
        new("Localization.LocaleFallback", "Locale fallback", "Locale", "Fallback", "Coordinates localization and time zones for Locale fallback."),
        new("Localization.LocaleFormatting", "Locale formatting", "Locale", "Formatting", "Coordinates localization and time zones for Locale formatting."),
        new("Localization.LocaleMapping", "Locale mapping", "Locale", "Mapping", "Coordinates localization and time zones for Locale mapping."),
        new("Localization.LocaleScheduling", "Locale scheduling", "Locale", "Scheduling", "Coordinates localization and time zones for Locale scheduling."),
        new("Localization.LocaleValidation", "Locale validation", "Locale", "Validation", "Coordinates localization and time zones for Locale validation."),
        new("Localization.LanguageCatalog", "Language catalog", "Language", "Catalog", "Coordinates localization and time zones for Language catalog."),
        new("Localization.LanguageFallback", "Language fallback", "Language", "Fallback", "Coordinates localization and time zones for Language fallback."),
        new("Localization.LanguageFormatting", "Language formatting", "Language", "Formatting", "Coordinates localization and time zones for Language formatting."),
        new("Localization.LanguageMapping", "Language mapping", "Language", "Mapping", "Coordinates localization and time zones for Language mapping."),
        new("Localization.LanguageScheduling", "Language scheduling", "Language", "Scheduling", "Coordinates localization and time zones for Language scheduling."),
        new("Localization.LanguageValidation", "Language validation", "Language", "Validation", "Coordinates localization and time zones for Language validation."),
        new("Localization.TerritoryCatalog", "Territory catalog", "Territory", "Catalog", "Coordinates localization and time zones for Territory catalog."),
        new("Localization.TerritoryFallback", "Territory fallback", "Territory", "Fallback", "Coordinates localization and time zones for Territory fallback."),
        new("Localization.TerritoryFormatting", "Territory formatting", "Territory", "Formatting", "Coordinates localization and time zones for Territory formatting."),
        new("Localization.TerritoryMapping", "Territory mapping", "Territory", "Mapping", "Coordinates localization and time zones for Territory mapping."),
        new("Localization.TerritoryScheduling", "Territory scheduling", "Territory", "Scheduling", "Coordinates localization and time zones for Territory scheduling."),
        new("Localization.TerritoryValidation", "Territory validation", "Territory", "Validation", "Coordinates localization and time zones for Territory validation."),
        new("Localization.CurrencyCatalog", "Currency catalog", "Currency", "Catalog", "Coordinates localization and time zones for Currency catalog."),
        new("Localization.CurrencyFallback", "Currency fallback", "Currency", "Fallback", "Coordinates localization and time zones for Currency fallback."),
        new("Localization.CurrencyFormatting", "Currency formatting", "Currency", "Formatting", "Coordinates localization and time zones for Currency formatting."),
        new("Localization.CurrencyMapping", "Currency mapping", "Currency", "Mapping", "Coordinates localization and time zones for Currency mapping."),
        new("Localization.CurrencyScheduling", "Currency scheduling", "Currency", "Scheduling", "Coordinates localization and time zones for Currency scheduling."),
        new("Localization.CurrencyValidation", "Currency validation", "Currency", "Validation", "Coordinates localization and time zones for Currency validation."),
        new("Localization.TimeZoneCatalog", "TimeZone catalog", "TimeZone", "Catalog", "Coordinates localization and time zones for TimeZone catalog."),
        new("Localization.TimeZoneFallback", "TimeZone fallback", "TimeZone", "Fallback", "Coordinates localization and time zones for TimeZone fallback."),
        new("Localization.TimeZoneFormatting", "TimeZone formatting", "TimeZone", "Formatting", "Coordinates localization and time zones for TimeZone formatting."),
        new("Localization.TimeZoneMapping", "TimeZone mapping", "TimeZone", "Mapping", "Coordinates localization and time zones for TimeZone mapping."),
        new("Localization.TimeZoneScheduling", "TimeZone scheduling", "TimeZone", "Scheduling", "Coordinates localization and time zones for TimeZone scheduling."),
        new("Localization.TimeZoneValidation", "TimeZone validation", "TimeZone", "Validation", "Coordinates localization and time zones for TimeZone validation."),
    };
}