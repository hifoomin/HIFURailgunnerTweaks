using HRGT.Misc;
using R2API;

namespace HRGT.Skills
{
    public class Supercharge : TweakBase
    {
        public static float Damage;
        public static float CritDamage;
        public static float PiercingDamage;
        public static float ProcCoefficient;
        public static float HopooBalance;
        public static float Radius;

        public override string Name => ": Special : Supercharge";

        public override string SkillToken => "special";

        public override string DescText => "Fire a <style=cIsDamage>piercing</style> round for <style=cIsDamage>" + d(Damage) + "-" + d((Damage + ScopeAndReload.Damage) * CritDamage * 2) + " damage</style>. Afterwards, <style=cIsHealth>all your weapons are disabled</style> for <style=cIsHealth>" + HopooBalance + "</style> seconds.";

        public override void Init()
        {
            Damage = ConfigOption(17.5f, "Damage", "Decimal. Vanilla is 40");
            CritDamage = ConfigOption(2f, "Crit Damage Multiplier", "Decimal. Vanilla is 1.5");
            PiercingDamage = ConfigOption(1f, "Pierce Damage Falloff", "Decimal. Vanilla is 1. Higher values mean less piercing damage falloff, so more damage overall.");
            ProcCoefficient = ConfigOption(1.5f, "Proc Coefficient", "Vanilla is 3");
            HopooBalance = ConfigOption(5f, "Disable Duration", "Vanilla is 5");
            Radius = ConfigOption(2f, "Radius", "Vanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Railgunner.Weapon.BaseFireSnipe.OnEnter += BaseFireSnipe_OnEnter;
            On.EntityStates.Railgunner.Backpack.Offline.OnEnter += Offline_OnEnter;
            LanguageAPI.Add("RAILGUNNER_SNIPE_SUPER_DESCRIPTION", "Launch a super-charged projectile for <style=cIsDamage>" + d(Damage) + " damage</style>. Critical Strike damage is multiplied by <style=cIsDamage>1.5</style>.");
        }

        private void Offline_OnEnter(On.EntityStates.Railgunner.Backpack.Offline.orig_OnEnter orig, EntityStates.Railgunner.Backpack.Offline self)
        {
            self.baseDuration = 1 + HopooBalance;
            orig(self);
        }

        private void BaseFireSnipe_OnEnter(On.EntityStates.Railgunner.Weapon.BaseFireSnipe.orig_OnEnter orig, EntityStates.Railgunner.Weapon.BaseFireSnipe self)
        {
            if (self is EntityStates.Railgunner.Weapon.FireSnipeSuper)
            {
                self.damageCoefficient = Damage;
                self.piercingDamageCoefficientPerTarget = PiercingDamage;
                self.critDamageMultiplier = CritDamage;
                self.procCoefficient = ProcCoefficient;
                self.recoilAmplitudeY = 7f;
                self.bulletRadius = Radius;
            }
            orig(self);
        }
    }
}