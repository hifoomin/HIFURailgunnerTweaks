using HRGT.Misc;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HRGT.Skills
{
    public class M99Sniper : TweakBase
    {
        public static float Damage;
        public static float PiercingDamage;
        public static float ZoomFOV;

        public override string Name => ": Secondary : M99 Sniper";

        public override string SkillToken => "secondary";

        public override string DescText => "Activate your <style=cIsUtility>long-range scope</style>, highlighting <style=cIsDamage>Weak Points</style> and transforming your weapon into a piercing <style=cIsDamage>" + d(Damage) + "-" + d((Damage + ScopeAndReload.Damage) * 2) + " damage</style> railgun.";

        public override void Init()
        {
            Damage = ConfigOption(6f, "Damage", "Decimal. Vanilla is 10");
            PiercingDamage = ConfigOption(0.5f, "Pierce Damage Falloff", "Decimal. Vanilla is 0.5");
            ZoomFOV = ConfigOption(18f, "Zoom FOV", "Vanilla is 18. Lower value is more zoomed in");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Railgunner.Weapon.BaseFireSnipe.OnEnter += BaseFireSnipe_OnEnter;
            LanguageAPI.Add("RAILGUNNER_SNIPE_HEAVY_DESCRIPTION", "Launch a heavy projectile for <style=cIsDamage>" + d(Damage) + " damage</style>.");
            Changes();
        }

        private void BaseFireSnipe_OnEnter(On.EntityStates.Railgunner.Weapon.BaseFireSnipe.orig_OnEnter orig, EntityStates.Railgunner.Weapon.BaseFireSnipe self)
        {
            if (self is EntityStates.Railgunner.Weapon.FireSnipeHeavy)
            {
                self.damageCoefficient = Damage;
                self.piercingDamageCoefficientPerTarget = PiercingDamage;
                self.selfKnockbackForce = 0f;
                self.recoilAmplitudeY = 4f;
            }
            orig(self);
        }

        private void Changes()
        {
            var ccp = Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/DLC1/Railgunner/ccpRailgunnerScopeHeavy.asset").WaitForCompletion();
            ccp.data.fov.value = ZoomFOV;
        }
    }
}