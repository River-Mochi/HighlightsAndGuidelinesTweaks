// Mod.cs
// Entry point for Advanced Hover; registers locale + settings UI,
// registers keybindings, and ensures the ECS system is running.

namespace AdvancedHoverSystem
{
    using Colossal;               // IDictionarySource
    using Colossal.IO.AssetDatabase;
    using Colossal.Logging;       // ILog, LogManager
    using Game;                   // UpdateSystem
    using Game.Input;             // ProxyAction
    using Game.Modding;           // IMod
    using Game.SceneFlow;         // GameManager
    using HighlightsAndGuidelinesTweaks.Systems; // ModifyRenderingSettingsPrefabSystem

    public sealed class Mod : IMod
    {
        // ---- Mod meta ----
        public const string ModName      = "Advanced Hover";
        public const string VersionShort = "0.1.0";
        public static string supportedGameVersion = "1.3.5f1";

        // Shared logger for whole mod
        public static ILog log = LogManager
            .GetLogger("Advanced Hover")
            .SetShowsErrorsInUI(true);

        // Current settings instance
        public static Setting? Settings { get; private set; }

        // Key actions (F8 toggle, F9 cycle)
        public static ProxyAction? ToggleHoverAction { get; private set; }
        public static ProxyAction? CycleColorAction  { get; private set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info("Advanced Hover OnLoad");

            // Create settings and load persisted values
            var settings = new Setting(this);
            Settings = settings;

            AssetDatabase.global.LoadSettings(
                "ModsSettings/AdvancedHover/AdvancedHover",
                settings,
                new Setting(this)); // default fallback

            // Register locale BEFORE Options UI
            AddLocale("en-US", new LocaleEN(settings));

            // Register Options UI
            settings.RegisterInOptionsUI();

            // Register key bindings (CO docs requirement). :contentReference[oaicite:4]{index=4}
            settings.RegisterKeyBindings();

            // Get actions from settings; may be null if conflicts prevent activation
            ToggleHoverAction = settings.GetAction(Setting.kToggleHoverActionName);
            CycleColorAction  = settings.GetAction(Setting.kCycleColorActionName);

            if (ToggleHoverAction != null)
            {
                ToggleHoverAction.shouldBeEnabled = true;
            }
            else
            {
                log.Warn("ToggleHoverAction is null (keybinding may be disabled due to conflict).");
            }

            if (CycleColorAction != null)
            {
                CycleColorAction.shouldBeEnabled = true;
            }
            else
            {
                log.Warn("CycleColorAction is null (keybinding may be disabled due to conflict).");
            }

            // Ensure our ECS system exists and is enabled
            var system = updateSystem.World.GetOrCreateSystemManaged<ModifyRenderingSettingsPrefabSystem>();
            system.Enabled = true;

            log.Info($"Advanced Hover loaded. Version {VersionShort}, game ≥ {supportedGameVersion}.");
        }

        public void OnDispose()
        {
            log.Info("Advanced Hover OnDispose");

            Settings?.UnregisterInOptionsUI();
            Settings = null;

            ToggleHoverAction = null;
            CycleColorAction  = null;
        }

        private static void AddLocale(string localeId, IDictionarySource source)
        {
            var lm = GameManager.instance?.localizationManager;
            if (lm == null)
            {
                log.Warn($"LocalizationManager null; cannot add locale '{localeId}'.");
                return;
            }

            lm.AddSource(localeId, source);
        }
    }
}
