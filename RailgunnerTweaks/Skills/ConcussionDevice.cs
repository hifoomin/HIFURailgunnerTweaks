using RoR2.Skills;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFURailgunnerTweaks.Skills
{
    public class ConcussionDevice : TweakBase
    {
        public static float SelfKnockback;
        public static int Charges;
        public static float Cooldown;

        public override string Name => ": Utility : Concussion Device";

        public override string SkillToken => "utility";

        public override string DescText => "Throw out a device that <style=cIsUtility>pushes</style> you and all nearby enemies away. Can hold up to " + Charges + ".";

        public override void Init()
        {
            SelfKnockback = ConfigOption(3200f, "Self Force", "Vanilla is 4000. Higher value is more knockback");
            Charges = ConfigOption(2, "Charges", "Vanilla is 2");
            Cooldown = ConfigOption(6f, "Cooldown", "Vanilla is 6");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Railgunner.Weapon.BaseFireMine.OnEnter += BaseFireMine_OnEnter;
            Changes();
        }

        private void BaseFireMine_OnEnter(On.EntityStates.Railgunner.Weapon.BaseFireMine.orig_OnEnter orig, EntityStates.Railgunner.Weapon.BaseFireMine self)
        {
            if (self is EntityStates.Railgunner.Weapon.FireMineConcussive)
            {
                self.force = SelfKnockback;
            }
            orig(self);
        }

        private void Changes()
        {
            var sd = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/Railgunner/RailgunnerBodyFireMineConcussive.asset").WaitForCompletion();
            sd.baseMaxStock = Charges;
            sd.baseRechargeInterval = Cooldown;
        }
    }
}