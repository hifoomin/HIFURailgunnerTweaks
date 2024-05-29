using RoR2;
using R2API;
using UnityEngine;

namespace HIFURailgunnerTweaks.Misc
{
    public class ScopeAndReload : MiscBase
    {
        public static float MinimumReloadDamageBonus;
        public static float MaximumReloadDamageBonus;
        public static float MinimumReloadBarPercent;
        public static float MaximumReloadBarPercent;
        public static int MaximumSuccessfulReloads;
        public static float ScopeDurUp;
        public static float ScopeDurDown;
        public static bool ScaleWithAS;
        public override string Name => ":: Misc : Scope and Active Reload";

        public override void Init()
        {
            base.Init();
            MinimumReloadDamageBonus = ConfigOption(2.5f, "Minimum Successive Reload Damage Bonus", "Decimal. Vanilla is 5 Formula for Damage Increase per reload: (Maximum Damage Successive Reload Damage Bonus - Minimum Successive Reload Damage Bonus) / Maximum Successive Reloads");
            MaximumReloadDamageBonus = ConfigOption(10f, "Maximum Damage Successive Reload Damage Bonus", "Decimal. Vanilla is 5");
            MinimumReloadBarPercent = ConfigOption(0.015f, "Minimum Successive Reload Bar Duration", "Vanilla is 0.25. Formula for Actual Percent: (Reload Bar Duration / 1.5) * 100");
            MaximumReloadBarPercent = ConfigOption(0.15f, "Maximum Successive Reload Bar Duration", "Vanilla is 0.25. Formula for Actual Percent: (Reload Bar Duration / 1.5) * 100");
            MaximumSuccessfulReloads = ConfigOption(5, "Maximum Successive Reloads", "Vanilla is ??? Formula for Reload Bar Duration decrease per reload: (MaximumReloadBarPercent - MinimumReloadBarPercent) / Maximum Successful Reloads");
            ScopeDurUp = ConfigOption(0f, "Scope Duration Wind Up", "Vanilla is 0.1");
            ScopeDurDown = ConfigOption(0f, "Scope Duration Wind Down", "Vanilla is 0.2");
            ScaleWithAS = ConfigOption(false, "Scale Reload Bar Duration with Attack Speed?", "Vanilla is true");
        }

        public override void Hooks()
        {
            On.EntityStates.Railgunner.Scope.BaseWindUp.OnEnter += BaseWindUp_OnEnter;
            On.EntityStates.Railgunner.Scope.BaseWindDown.OnEnter += BaseWindDown_OnEnter;
            On.EntityStates.Railgunner.Reload.Boosted.OnEnter += Boosted_OnEnter;
            On.EntityStates.Railgunner.Reload.Reloading.OnEnter += Reloading_OnEnter;
            On.EntityStates.Railgunner.Reload.Reloading.AttemptBoost += Reloading_AttemptBoost;
            LanguageAPI.Add("RAILGUNNER_ACTIVE_RELOAD_DESCRIPTION", "Perfectly time your reload to recover faster and to boost the damage of your next shot by <style=cIsDamage>+" + d(MinimumReloadDamageBonus) + "-" + d(MaximumReloadDamageBonus) + "</style>.");
        }

        private bool Reloading_AttemptBoost(On.EntityStates.Railgunner.Reload.Reloading.orig_AttemptBoost orig, EntityStates.Railgunner.Reload.Reloading self)
        {
            var ret = orig(self);
            if (ret && self.hasAttempted)
            {
                if (self.characterBody.TryGetComponent<ReloadScalingComponent>(out var reloadScalingComponent))
                {
                    reloadScalingComponent.successfulReloadCounter = Mathf.Min(MaximumSuccessfulReloads, reloadScalingComponent.successfulReloadCounter + 1); // jank for now, idk why util remap ignores my cap
                }
            }
            else
            {
                if (self.characterBody.TryGetComponent<ReloadScalingComponent>(out var reloadScalingComponent))
                {
                    reloadScalingComponent.successfulReloadCounter = 0;
                }
            }
            return ret;
        }

        private void Reloading_OnEnter(On.EntityStates.Railgunner.Reload.Reloading.orig_OnEnter orig, EntityStates.Railgunner.Reload.Reloading self)
        {
            if (self.outer.TryGetComponent<ReloadScalingComponent>(out var reloadScalingComponent))
            {
                self.boostWindowDuration = Util.Remap(reloadScalingComponent.successfulReloadCounter, 0, MaximumSuccessfulReloads, MaximumReloadBarPercent, MinimumReloadBarPercent);
            }

            orig(self);

            if (ScaleWithAS == false)
            {
                // Main.HRGTLogger.LogError("scale with as is false");
                self.duration = (self.boostWindowDelay + self.boostWindowDuration) + (self.baseDuration - (self.boostWindowDelay + self.boostWindowDuration));
                self.adjustedBoostWindowDelay = Mathf.Min(self.boostWindowDelay / self.baseDuration * self.duration, self.boostWindowDelay);
                self.adjustedBoostWindowDuration = Mathf.Max((self.boostWindowDelay + self.boostWindowDuration) / self.baseDuration * self.duration, (self.boostWindowDelay + self.boostWindowDuration)) - self.adjustedBoostWindowDelay;
                // Main.HRGTLogger.LogError("duration is " + self.duration);
                // Main.HRGTLogger.LogError("boost window delay is " + self.boostWindowDelay);
                // Main.HRGTLogger.LogError("boost window duration is " + self.boostWindowDuration);
                // Main.HRGTLogger.LogError("base duration is " + self.baseDuration);
                // Main.HRGTLogger.LogError("adjusted boost window delay is " + self.adjustedBoostWindowDelay);
                // Main.HRGTLogger.LogError("adjusted boost window duration is " + self.adjustedBoostWindowDuration);
            }
        }

        private void Boosted_OnEnter(On.EntityStates.Railgunner.Reload.Boosted.orig_OnEnter orig, EntityStates.Railgunner.Reload.Boosted self)
        {
            if (self.outer.TryGetComponent<ReloadScalingComponent>(out var reloadScalingComponent))
            {
                var increase = (MaximumReloadDamageBonus - MinimumReloadDamageBonus) / MaximumSuccessfulReloads;
                self.bonusDamageCoefficient = Util.Remap(reloadScalingComponent.successfulReloadCounter, 0, MaximumSuccessfulReloads, MinimumReloadDamageBonus - increase, MaximumReloadDamageBonus);
            }
            orig(self);
        }

        private void BaseWindDown_OnEnter(On.EntityStates.Railgunner.Scope.BaseWindDown.orig_OnEnter orig, EntityStates.Railgunner.Scope.BaseWindDown self)
        {
            self.baseDuration = ScopeDurUp;
            if (self is EntityStates.Railgunner.Scope.WindDownScopeHeavy || self is EntityStates.Railgunner.Scope.WindDownScopeLight)
            {
                self.baseDuration = ScopeDurDown;
            }
            orig(self);
        }

        private void BaseWindUp_OnEnter(On.EntityStates.Railgunner.Scope.BaseWindUp.orig_OnEnter orig, EntityStates.Railgunner.Scope.BaseWindUp self)
        {
            self.baseDuration = ScopeDurUp;
            if (self is EntityStates.Railgunner.Scope.WindUpScopeHeavy || self is EntityStates.Railgunner.Scope.WindUpScopeLight)
            {
                self.baseDuration = ScopeDurUp;
            }
            orig(self);
        }
    }

    public class ReloadScalingComponent : MonoBehaviour
    {
        public int successfulReloadCounter = 0;
    }
}