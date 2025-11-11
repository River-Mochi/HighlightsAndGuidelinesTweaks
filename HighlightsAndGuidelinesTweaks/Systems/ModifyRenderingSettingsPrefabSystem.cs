// ModifyRenderingSettingsPrefabSystem.cs
// Applies AdvancedHoverSystem settings to RenderingSettings prefab & runtime.
// Runs once after each city finishes loading, and re-applies only when settings change.

namespace HighlightsAndGuidelinesTweaks.Systems
{
    using Colossal.Logging;
    using Game;
    using Game.Prefabs;
    using Game.Rendering;
    using Unity.Entities;
    using UnityEngine;

    /// <summary>
    /// Applies rendering and hover outline settings to the RenderingSettings prefab entity
    /// and runtime RenderingSettingsData, driven by AdvancedHoverSystem.Setting.
    /// </summary>
    public partial class ModifyRenderingSettingsPrefabSystem : GameSystemBase
    {
        public readonly PrefabID RenderingSettingsPrefab =
            new PrefabID(nameof(RenderingSettingsPrefab), "RenderingSettings");

        private ILog m_Log = null!;
        private PrefabSystem m_PrefabSystem = null!;
        private EntityQuery m_RuntimeRenderingSettingsQuery;

        // Cache of vanilla hover color (RGB kept; alpha driven by settings)
        private Color m_VanillaHoverColor;
        private bool m_HasVanilla;

        // Last applied values to avoid unnecessary work
        private bool m_LastDisabled;
        private AdvancedHoverSystem.HoverColorPreset m_LastPreset;
        private float m_LastBrightness;

        protected override void OnCreate()
        {
            base.OnCreate();

            m_Log = AdvancedHoverSystem.Mod.log;
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            m_RuntimeRenderingSettingsQuery = GetEntityQuery(ComponentType.ReadWrite<RenderingSettingsData>());
            Enabled = true;

            // Capture "vanilla" hover color once from the prefab if available
            if (m_PrefabSystem.TryGetPrefab(RenderingSettingsPrefab, out PrefabBase prefab)
                && m_PrefabSystem.TryGetEntity(prefab, out Entity prefabEntity)
                && EntityManager.HasComponent<RenderingSettingsData>(prefabEntity))
            {
                var data = EntityManager.GetComponentData<RenderingSettingsData>(prefabEntity);
                m_VanillaHoverColor = new Color(
                    data.m_HoveredColor.r,
                    data.m_HoveredColor.g,
                    data.m_HoveredColor.b,
                    1f);
                m_HasVanilla = true;
            }

            // Seed "last" values so first ApplyFromSettings will definitely run
            var s = AdvancedHoverSystem.Mod.Settings;
            bool currentDisabled = s?.DisableHoverOutline ?? false;
            var currentPreset = s?.HoverColor ?? AdvancedHoverSystem.HoverColorPreset.Purple;
            float currentBrightness = s?.HoverBrightness ?? 1.0f;

            m_LastDisabled = !currentDisabled;
            m_LastPreset = currentPreset == AdvancedHoverSystem.HoverColorPreset.Purple
                ? AdvancedHoverSystem.HoverColorPreset.Gray
                : AdvancedHoverSystem.HoverColorPreset.Purple;
            m_LastBrightness = -1f;

            m_Log.Info($"{nameof(ModifyRenderingSettingsPrefabSystem)}.{nameof(OnCreate)} complete.");
        }

        /// <summary>
        /// Per-frame: cheap check to see if settings changed; re-apply only when needed.
        /// This handles "player opens Options, changes color, goes back into city".
        /// </summary>
        protected override void OnUpdate()
        {
            ApplyFromSettings(force: false);
        }

        /// <summary>
        /// Called when a city has finished loading.
        /// We force-apply here so each loaded save gets correct settings.
        /// </summary>
        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            m_Log.Info($"{nameof(ModifyRenderingSettingsPrefabSystem)}.{nameof(OnGameLoadingComplete)} -> force apply.");
            ApplyFromSettings(force: true);
        }

        /// <summary>
        /// Set the RenderingSettingsData on the RenderingSettings prefab entity if present.
        /// (Helper kept in case we want to call it elsewhere.)
        /// </summary>
        public void SetRenderingSettingsData(RenderingSettingsData newRenderingSettingsData)
        {
            if (m_PrefabSystem.TryGetPrefab(RenderingSettingsPrefab, out PrefabBase prefab)
                && m_PrefabSystem.TryGetEntity(prefab, out Entity prefabEntity)
                && EntityManager.HasComponent<RenderingSettingsData>(prefabEntity))
            {
                EntityManager.SetComponentData(prefabEntity, newRenderingSettingsData);
            }

            m_Log.Info($"{nameof(ModifyRenderingSettingsPrefabSystem)}.{nameof(SetRenderingSettingsData)} complete.");
        }

        /// <summary>
        /// Set GuideLineSettingsData on the RenderingSettings prefab entity if present.
        /// (Not currently used by AdvancedHoverSystem, but preserved for future tweaks.)
        /// </summary>
        public void SetGuideLineSettingsData(GuideLineSettingsData newGuideLinesSettingsData)
        {
            if (m_PrefabSystem.TryGetPrefab(RenderingSettingsPrefab, out PrefabBase prefab)
                && m_PrefabSystem.TryGetEntity(prefab, out Entity prefabEntity)
                && EntityManager.HasComponent<GuideLineSettingsData>(prefabEntity))
            {
                EntityManager.SetComponentData(prefabEntity, newGuideLinesSettingsData);
            }

            m_Log.Info($"{nameof(ModifyRenderingSettingsPrefabSystem)}.{nameof(SetGuideLineSettingsData)} complete.");
        }

        /// <summary>
        /// Core logic: read AdvancedHoverSystem.Mod.Settings, compute hover color,
        /// and push it to prefab + runtime RenderingSettingsData, and toggle overlay.
        /// </summary>
        private void ApplyFromSettings(bool force)
        {
            var s = AdvancedHoverSystem.Mod.Settings;
            if (s == null)
            {
                return;
            }

            bool disabled = s.DisableHoverOutline;
            var preset = s.HoverColor;
            float brightness = Mathf.Clamp(s.HoverBrightness, 0f, 2f);

            // Skip work if nothing changed and we aren't forced
            if (!force
                && disabled == m_LastDisabled
                && preset == m_LastPreset
                && Mathf.Abs(brightness - m_LastBrightness) < 0.0001f)
            {
                return;
            }

            // Get prefab entity
            if (!m_PrefabSystem.TryGetPrefab(RenderingSettingsPrefab, out PrefabBase prefab)
                || !m_PrefabSystem.TryGetEntity(prefab, out Entity prefabEntity)
                || !EntityManager.HasComponent<RenderingSettingsData>(prefabEntity))
            {
                m_Log.Warn("RenderingSettings prefab not found; cannot apply hover settings.");
                return;
            }

            var data = EntityManager.GetComponentData<RenderingSettingsData>(prefabEntity);

            // Compute target hover color from preset + brightness
            Color target = GetPresetRgb(preset);
            target.r = Mathf.Clamp01(target.r * brightness);
            target.g = Mathf.Clamp01(target.g * brightness);
            target.b = Mathf.Clamp01(target.b * brightness);

            // Alpha: outline is mostly alpha-insensitive; keep minimal non-zero when enabled
            target.a = disabled ? 0f : 0.10f;

            // Apply to prefab
            if (!ColorsApproximatelyEqual(data.m_HoveredColor, target) || force)
            {
                data.m_HoveredColor = target;
                EntityManager.SetComponentData(prefabEntity, data);
            }

            // Apply to any runtime RenderingSettingsData entities (live city)
            if (!m_RuntimeRenderingSettingsQuery.IsEmptyIgnoreFilter)
            {
                using (var entities = m_RuntimeRenderingSettingsQuery.ToEntityArray(Unity.Collections.Allocator.Temp))
                {
                    for (int i = 0; i < entities.Length; i++)
                    {
                        var e = entities[i];
                        var runtimeData = EntityManager.GetComponentData<RenderingSettingsData>(e);
                        if (!ColorsApproximatelyEqual(runtimeData.m_HoveredColor, target) || force)
                        {
                            runtimeData.m_HoveredColor = target;
                            EntityManager.SetComponentData(e, runtimeData);
                        }
                    }
                }
            }

            // Hide/show overlay to suppress outlines drawn by EditorGizmoSystem when disabled
            var renderingSystem = World.GetExistingSystemManaged<RenderingSystem>();
            if (renderingSystem != null)
            {
                renderingSystem.hideOverlay = disabled;
            }

            // Update last-applied cache
            m_LastDisabled = disabled;
            m_LastPreset = preset;
            m_LastBrightness = brightness;

            m_Log.Info(
                $"Applied hover settings: disabled={disabled}, preset={preset}, brightness={brightness:F2}, alpha={target.a:F2}");
        }

        private Color GetPresetRgb(AdvancedHoverSystem.HoverColorPreset preset)
        {
            switch (preset)
            {
                case AdvancedHoverSystem.HoverColorPreset.Purple:
                    return new Color(0.50f, 0.25f, 0.70f, 1f);
                case AdvancedHoverSystem.HoverColorPreset.White:
                    return new Color(1f, 1f, 1f, 1f);
                case AdvancedHoverSystem.HoverColorPreset.Gray:
                    return new Color(0.70f, 0.70f, 0.70f, 1f);
                case AdvancedHoverSystem.HoverColorPreset.Beige:
                    return new Color(0.82f, 0.75f, 0.62f, 1f);
                case AdvancedHoverSystem.HoverColorPreset.Green:
                    return new Color(0.25f, 0.85f, 0.35f, 1f);
                case AdvancedHoverSystem.HoverColorPreset.Vanilla:
                default:
                    if (m_HasVanilla)
                    {
                        return new Color(
                            m_VanillaHoverColor.r,
                            m_VanillaHoverColor.g,
                            m_VanillaHoverColor.b,
                            1f);
                    }
                    // Fallback "vanilla-ish" blue if capture failed
                    return new Color(0.25f, 0.35f, 1f, 1f);
            }
        }

        private static bool ColorsApproximatelyEqual(Color a, Color b)
        {
            const float eps = 0.0005f;
            return Mathf.Abs(a.r - b.r) < eps
                && Mathf.Abs(a.g - b.g) < eps
                && Mathf.Abs(a.b - b.b) < eps
                && Mathf.Abs(a.a - b.a) < eps;
        }
    }
}
