// LocaleEN.cs
// English locale entries for Advanced Hover Options UI + keybindings.

namespace AdvancedHoverSystem
{
    using Colossal;                    // IDictionarySource
    using System.Collections.Generic;  // Dictionary, IList

    public sealed class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // Mod name in Options list
                { m_Setting.GetSettingsLocaleID(), "Advanced Hover" },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "About"   },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsOutlineGroup),  "Hover Outline" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsKeybindsGroup), "Keybinds"      },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGroup),       "About"         },

                // ---- Actions tab: Hover Outline ----
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DisableHoverOutline)),
                  "Disable hover outline" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DisableHoverOutline)),
                  "When checked, the hover/selection outline is hidden (alpha near zero)." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HoverColor)),
                  "Hover color preset" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.HoverColor)),
                  "Choose the outline color preset. \"Vanilla\" restores the game's default tone." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HoverBrightness)),
                  "Outline brightness" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.HoverBrightness)),
                  "Scales color intensity (0 = black, 1 = normal, 2 = bright)." },

                // ---- Actions tab: Keybinds ----
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ToggleHoverBinding)),
                  "Toggle hover outline (F8)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ToggleHoverBinding)),
                  "Keybinding to toggle the hover outline on/off." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CycleColorBinding)),
                  "Cycle hover color preset (F9)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CycleColorBinding)),
                  "Keybinding to cycle through hover color presets." },

                // ---- About tab ----
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ModNameDisplay)),
                  "Mod name" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ModNameDisplay)),
                  "Display name of this mod." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionDisplay)),
                  "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionDisplay)),
                  "Current mod version." },
            };
        }

        public void Unload()
        {
            // Nothing to dispose.
        }
    }
}
