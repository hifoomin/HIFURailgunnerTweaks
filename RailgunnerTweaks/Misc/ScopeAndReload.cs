using R2API;
using System;
using UnityEngine;

namespace HRGT.Misc
{
    public class ScopeAndReload : MiscBase
    {
        public static float Damage;
        public static float ReloadBarPercent;
        public static float ScopeDurUp;
        public static float ScopeDurDown;
        public static bool ScaleWithAS;
        public override string Name => ":: Misc : Scope and Active Reload";

        public override void Init()
        {
            base.Init();
            Damage = ConfigOption(5f, "Damage Bonus", "Decimal. Vanilla is 5");
            ReloadBarPercent = ConfigOption(0.13f, "Reload Bar Duration", "Vanilla is 0.25. Formula: (Reload Bar Duration / 1.5) * 100 for actual percent");
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
            LanguageAPI.Add("RAILGUNNER_ACTIVE_RELOAD_DESCRIPTION", "Perfectly time your reload to recover faster and to boost the damage of your next shot by <style=cIsDamage>+" + d(Damage) + "</style>.");
        }

        private void Reloading_OnEnter(On.EntityStates.Railgunner.Reload.Reloading.orig_OnEnter orig, EntityStates.Railgunner.Reload.Reloading self)
        {
            self.boostWindowDuration = ReloadBarPercent;

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
            self.bonusDamageCoefficient = Damage;
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
}