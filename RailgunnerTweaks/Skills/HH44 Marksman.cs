using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HRGT.Skills
{
    public class HH44Marksman : TweakBase
    {
        public static float Damage;
        public static float PiercingDamage;
        public static float AttackRate;
        public static float ZoomFOV;

        public override string Name => ": Secondary :: HH44 Marksman";

        public override string SkillToken => "secondary_alt";

        public override string DescText => "Activate your <style=cIsUtility>short-range scope</style>, highlighting <style=cIsDamage>Weak Points</style> and transforming your weapon into a quick <style=cIsDamage>" + d(Damage) + "-" + d(Damage * 2) + " damage</style> railgun.";

        public override void Init()
        {
            Damage = ConfigOption(4f, "Damage", "Decimal. Vanilla is 4");
            PiercingDamage = ConfigOption(0.5f, "Pierce Damage Falloff", "Decimal. Vanilla is 0.5. Higher values mean less piercing damage falloff, so more damage overall.");
            AttackRate = ConfigOption(2f, "Attacks Per Second", "Vanilla is 2");
            ZoomFOV = ConfigOption(30f, "Zoom FOV", "Vanilla is 30. Lower value is more zoomed in");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Railgunner.Weapon.BaseFireSnipe.OnEnter += BaseFireSnipe_OnEnter;
            LanguageAPI.Add("RAILGUNNER_SNIPE_LIGHT_DESCRIPTION", "Launch a light projectile for <style=cIsDamage>" + d(Damage) + " damage</style>.");
            Changes();
        }

        private void BaseFireSnipe_OnEnter(On.EntityStates.Railgunner.Weapon.BaseFireSnipe.orig_OnEnter orig, EntityStates.Railgunner.Weapon.BaseFireSnipe self)
        {
            if (self is EntityStates.Railgunner.Weapon.FireSnipeLight)
            {
                self.damageCoefficient = Damage;
                self.piercingDamageCoefficientPerTarget = PiercingDamage;
                self.selfKnockbackForce = 0f;
                self.baseDuration = 1 / AttackRate;
                self.recoilAmplitudeY = 0.6f;
            }
            orig(self);
        }

        private void Changes()
        {
            var ccp = Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/DLC1/Railgunner/ccpRailgunnerScopeLight.asset").WaitForCompletion();
            ccp.data.fov.value = ZoomFOV;
        }
    }
}