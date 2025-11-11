// Setting.cs
// Mod settings + Options UI layout for Advanced Hover.

namespace AdvancedHoverSystem
{
    using Colossal.IO.AssetDatabase;   // [FileLocation]
    using Game.Modding;                // IMod
    using Game.Settings;               // ModSetting + [SettingsUI*]
    using Game.UI.Widgets;             // DropdownItem<T>

    // Keep enum here for simplicity
    public enum HoverColorPreset
    {
        Vanilla = 0,
        White   = 1,
        Gray    = 2,
        Purple  = 3,
        Beige   = 4,
        Green   = 5,
    }

    [FileLocation("ModsSettings/AdvancedHover/AdvancedHover")]
    [SettingsUITabOrder(MainTab, AboutTab)]
    [SettingsUIGroupOrder(MainGroup)]
    [SettingsUIShowGroupName(MainGroup)]
    public sealed class Setting : ModSetting
    {
        // ---- UI structure ----
        public const string MainTab   = "Main";
        public const string AboutTab  = "About";
        public const string MainGroup = "HoverOutline";

        public Setting(IMod mod)
            : base(mod)
        {
        }

        // Checkbox: hide outline when true
        [SettingsUISection(MainTab, MainGroup)]
        public bool DisableHoverOutline { get; set; } = false;

        // Dropdown: hover color preset
        [SettingsUISection(MainTab, MainGroup)]
        [SettingsUIDropdown(typeof(Setting), nameof(GetHoverColorChoices))]
        public HoverColorPreset HoverColor { get; set; } = HoverColorPreset.Purple;

        // Slider: brightness multiplier 0..2 (clamped later)
        [SettingsUISection(MainTab, MainGroup)]
        [SettingsUISlider]
        public float HoverBrightness { get; set; } = 1.00f;

        // Opacity intentionally omitted in Phase 1:
        // gizmo outline is mostly alpha-insensitive and we keep alpha minimal inside the system.

        // Populate the dropdown (value drives the property; display text comes from Locale)
        public static DropdownItem<HoverColorPreset>[] GetHoverColorChoices() =>
            new[]
            {
                new DropdownItem<HoverColorPreset>
                {
                    value = HoverColorPreset.Vanilla,
                    displayName = "Vanilla",
                },
                new DropdownItem<HoverColorPreset>
                {
                    value = HoverColorPreset.White,
                    displayName = "White",
                },
                new DropdownItem<HoverColorPreset>
                {
                    value = HoverColorPreset.Gray,
                    displayName = "Gray",
                },
                new DropdownItem<HoverColorPreset>
                {
                    value = HoverColorPreset.Purple,
                    displayName = "Purple",
                },
                new DropdownItem<HoverColorPreset>
                {
                    value = HoverColorPreset.Beige,
                    displayName = "Beige",
                },
                new DropdownItem<HoverColorPreset>
                {
                    value = HoverColorPreset.Green,
                    displayName = "Green",
                },
            };

        public override void SetDefaults()
        {
            DisableHoverOutline = false;          // hover ON by default
            HoverColor = HoverColorPreset.Purple;
            HoverBrightness = 1.00f;
            // opacity not used in Phase 1
        }
    }
}
