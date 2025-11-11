// Mod.cs
// Entry point for Advanced Hover; registers locale + settings UI and ensures the ECS system is running.

namespace AdvancedHoverSystem
{
    using Colossal;               // IDictionarySource
    using Colossal.IO.AssetDatabase;
    using Colossal.Logging;       // ILog, LogManager
    using Game;                   // UpdateSystem
    using Game.Modding;           // IMod
    using Game.SceneFlow;         // GameManager
    using HighlightsAndGuidelinesTweaks.Systems; // ModifyRenderingSettingsPrefabSystem

    /// <summary>
    /// Entry point: registers locale + settings UI; ensures the system is active.
    /// </summary>
    public sealed class Mod : IMod
    {
        // Single logger used by the whole mod (including systems).
        public static ILog log = LogManager
            .GetLogger("Advanced Hover")
            .SetShowsErrorsInUI(true);

        public static string supportedGameVersion = "1.3.5f1";

        public static Setting? Settings { get; private set; }

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

            // Options UI
            settings.RegisterInOptionsUI();

            // Ensure our ECS system exists and is enabled
            var system = updateSystem.World.GetOrCreateSystemManaged<ModifyRenderingSettingsPrefabSystem>();
            system.Enabled = true;

            log.Info("Advanced Hover loaded.");
        }

        public void OnDispose()
        {
            log.Info("Advanced Hover OnDispose");

            // Only undo what this mod owns explicitly.
            Settings?.UnregisterInOptionsUI();
            Settings = null;
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
