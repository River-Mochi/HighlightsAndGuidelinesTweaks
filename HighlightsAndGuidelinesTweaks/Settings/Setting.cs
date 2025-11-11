// Setting.cs
// Mod settings + Options UI layout + keybinding definitions for Advanced Hover.

namespace AdvancedHoverSystem
{
    using Colossal.IO.AssetDatabase;   // [FileLocation]
    using Game.Modding;                // IMod, ModSetting
    using Game.Settings;               // SettingsUI*, ModSetting
    using Game.UI.Widgets;             // [SettingsUIMultilineText]
    using Game.Input;                  // ProxyBinding, ActionType, SettingsUI*Action/Binding
    using UnityEngine.InputSystem;     // Key

    // Color presets used by the hover outline system.
    public enum HoverColorPreset
    {
        Vanilla = 0,
        White   = 1,
        Gray    = 2,
        Purple  = 3,
        Beige   = 4,
        Green   = 5,
    }

    // Declare keyboard actions for our bindings (F8/F9).
    // NOTE: These attributes go on the settings class itself. :contentReference[oaicite:1]{index=1}
    [SettingsUIKeyboardAction(kToggleHoverActionName, ActionType.Button)]
    [SettingsUIKeyboardAction(kCycleColorActionName, ActionType.Button)]
    [FileLocation("ModsSettings/AdvancedHover/AdvancedHover")]
    [SettingsUITabOrder(ActionsTab, AboutTab)]
    [SettingsUIGroupOrder(
        ActionsOutlineGroup,
        ActionsKeybindsGroup,
        AboutInfoGroup
    )]
    [SettingsUIShowGroupName(
        ActionsOutlineGroup,
        ActionsKeybindsGroup,
        AboutInfoGroup
    )]
    public sealed class Setting : ModSetting
    {
        // ---- Tabs ----
        public const string ActionsTab = "Actions";
        public const string AboutTab   = "About";

        // ---- Groups ----
        public const string ActionsOutlineGroup  = "HoverOutline";
        public const string ActionsKeybindsGroup = "Keybinds";
        public const string AboutInfoGroup       = "AboutInfo";

        // ---- Action names for input system ----
        public const string kToggleHoverActionName = "AdvancedHover.ToggleHoverOutline";
        public const string kCycleColorActionName  = "AdvancedHover.CycleHoverColor";

        public Setting(IMod mod)
            : base(mod)
        {
        }

        // =====================================================================
        // ACTIONS TAB → Hover outline behavior
        // =====================================================================

        // Checkbox: hide outline when true
        [SettingsUISection(ActionsTab, ActionsOutlineGroup)]
        public bool DisableHoverOutline { get; set; } = false;

        // Dropdown: simple enum pattern (Method A). DO NOT add [SettingsUIDropdown]. :contentReference[oaicite:2]{index=2}
        [SettingsUISection(ActionsTab, ActionsOutlineGroup)]
        public HoverColorPreset HoverColor { get; set; } = HoverColorPreset.Purple;

        // Slider: brightness multiplier 0..2 (clamped later in system)
        [SettingsUISection(ActionsTab, ActionsOutlineGroup)]
        [SettingsUISlider]
        public float HoverBrightness { get; set; } = 1.00f;

        // =====================================================================
        // ACTIONS TAB → Keybinds (F8 toggle, F9 cycle color)
        // =====================================================================

        // F8: toggle DisableHoverOutline
        [SettingsUISection(ActionsTab, ActionsKeybindsGroup)]
        [SettingsUIKeyboardBinding(Key.F8, kToggleHoverActionName)]
        public ProxyBinding ToggleHoverBinding { get; set; }

        // F9: cycle through HoverColorPreset values
        [SettingsUISection(ActionsTab, ActionsKeybindsGroup)]
        [SettingsUIKeyboardBinding(Key.F9, kCycleColorActionName)]
        public ProxyBinding CycleColorBinding { get; set; }

        // =====================================================================
        // ABOUT TAB → Mod name + version display
        // =====================================================================

        [SettingsUISection(AboutTab, AboutInfoGroup)]
        public string ModNameDisplay => Mod.ModName;

        [SettingsUISection(AboutTab, AboutInfoGroup)]
        public string VersionDisplay => Mod.VersionShort;

        // =====================================================================
        // Defaults
        // =====================================================================

        public override void SetDefaults()
        {
            DisableHoverOutline = false;          // hover ON by default
            HoverColor = HoverColorPreset.Purple;
            HoverBrightness = 1.00f;
            // keybindings: defaults are defined by attributes; no manual values here
        }
    }
}
