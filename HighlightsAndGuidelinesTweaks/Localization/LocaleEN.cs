// LocaleEN.cs
// English locale entries for Advanced Hover Options UI.

namespace AdvancedHoverSystem
{
    using Colossal;                    // IDictionarySource
    using System.Collections.Generic;  // Dictionary, IList

    /// <summary>
    /// English locale entries for the Options UI.
    /// </summary>
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
                { m_Setting.GetOptionTabLocaleID(Setting.MainTab),  "Main"  },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "About" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.MainGroup), "Hover Outline" },

                // Checkbox
                {
                    m_Setting.GetOptionLabelLocaleID(nameof(Setting.DisableHoverOutline)),
                    "Disable hover outline"
                },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.DisableHoverOutline)),
                    "When checked, the hover/selection outline is hidden (alpha near zero)."
                },

                // Dropdown
                {
                    m_Setting.GetOptionLabelLocaleID(nameof(Setting.HoverColor)),
                    "Hover color"
                },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.HoverColor)),
                    "Choose the outline color. “Vanilla” restores the game’s default."
                },

                // Slider (brightness)
                {
                    m_Setting.GetOptionLabelLocaleID(nameof(Setting.HoverBrightness)),
                    "Outline brightness"
                },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.HoverBrightness)),
                    "Scales color intensity (0 = black, 1 = normal, 2 = bright)."
                },

                // About tab could get more text later if you want (version, author, links, etc.).
            };
        }

        public void Unload()
        {
            // Nothing to dispose.
        }
    }
}
