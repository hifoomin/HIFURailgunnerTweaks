using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HRGT.Skills
{
    public class XQRSmartRoundSystem : TweakBase
    {
        public static float LookCone;
        public static float Lifetime;
        public static float SelfForce;

        public override string Name => ": Primary : XQR Smart Round System";

        public override string SkillToken => "primary";

        public override string DescText => "Fire aggressive tracking rounds for <style=cIsDamage>100% damage</style>.";

        public override void Init()
        {
            LookCone = ConfigOption(25f, "Max Tracking Angle", "Vanilla is 90");
            Lifetime = ConfigOption(0.7f, "Lifetime", "Vanilla is 0.4");
            SelfForce = ConfigOption(150f, "Self Force", "Vanilla is 300. Higher value is more knockback");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Railgunner.Weapon.FirePistol.OnEnter += FirePistol_OnEnter;
            Changes();
        }

        private void FirePistol_OnEnter(On.EntityStates.Railgunner.Weapon.FirePistol.orig_OnEnter orig, EntityStates.Railgunner.Weapon.FirePistol self)
        {
            self.baseInaccuracyDegrees = 0.4f;
            self.selfKnockbackForce = SelfForce;
            orig(self);
        }

        private void Changes()
        {
            var p = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerPistolProjectile.prefab").WaitForCompletion();
            p.GetComponent<ProjectileSimple>().lifetime = Lifetime;
            p.GetComponent<ProjectileSteerTowardTarget>().rotationSpeed = 9999f;
            var pd = p.GetComponent<ProjectileDirectionalTargetFinder>();
            pd.lookCone = LookCone;
            pd.targetSearchInterval = 0.05f;
        }
    }
}