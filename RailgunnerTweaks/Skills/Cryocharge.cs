using HRGT.Misc;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HRGT.Skills
{
    public class Cryocharge : TweakBase
    {
        public static float Damage;
        public static float CritDamage;
        public static float PiercingDamage;
        public static float ProcCoefficient;

        public override string Name => ": Special :: Cryocharge";

        public override string SkillToken => "special_alt";

        public override string DescText => "<style=cIsUtility>Freezing</style>. Fire a <style=cIsDamage>piercing</style> round for <style=cIsDamage>" + d(Damage) + "-" + d(Damage * CritDamage * 2) + " damage</style>.";

        public override void Init()
        {
            Damage = ConfigOption(20f, "Damage", "Decimal. Vanilla is 20");
            CritDamage = ConfigOption(1f, "Crit Damage Multiplier", "Decimal. Vanilla is 1");
            PiercingDamage = ConfigOption(1f, "Pierce Damage Falloff", "Decimal. Vanilla is 1");
            ProcCoefficient = ConfigOption(1.5f, "Proc Coefficient", "Vanilla is 1.5");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Railgunner.Weapon.BaseFireSnipe.OnEnter += BaseFireSnipe_OnEnter;
            LanguageAPI.Add("RAILGUNNER_SNIPE_CRYO_DESCRIPTION", "<style=cIsUtility>Freezing</style>. Launch a super-cooled projectile for <style=cIsDamage>" + d(Damage) + " damage</style>.");
        }

        private void BaseFireSnipe_OnEnter(On.EntityStates.Railgunner.Weapon.BaseFireSnipe.orig_OnEnter orig, EntityStates.Railgunner.Weapon.BaseFireSnipe self)
        {
            if (self is EntityStates.Railgunner.Weapon.FireSnipeCryo)
            {
                self.damageCoefficient = Damage;
                self.piercingDamageCoefficientPerTarget = PiercingDamage;
                self.critDamageMultiplier = CritDamage;
                self.procCoefficient = ProcCoefficient;
            }
            orig(self);
        }
    }
}