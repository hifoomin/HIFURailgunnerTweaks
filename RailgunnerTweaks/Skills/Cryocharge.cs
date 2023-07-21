using HIFURailgunnerTweaks.Misc;
using R2API;

namespace HIFURailgunnerTweaks.Skills
{
    public class Cryocharge : TweakBase
    {
        public static float Damage;
        public static float CritDamage;
        public static float PiercingDamage;
        public static float ProcCoefficient;
        public static float Radius;

        public override string Name => ": Special :: Cryocharge";

        public override string SkillToken => "special_alt";

        public override string DescText => "<style=cIsUtility>Freezing</style>. Fire a <style=cIsDamage>piercing</style> round for <style=cIsDamage>" + d(Damage) + "-" + d((Damage + ScopeAndReload.Damage) * CritDamage * 2) + " damage</style>.";

        public override void Init()
        {
            Damage = ConfigOption(20f, "Damage", "Decimal. Vanilla is 20");
            CritDamage = ConfigOption(1f, "Crit Damage Multiplier", "Decimal. Vanilla is 1");
            PiercingDamage = ConfigOption(1f, "Pierce Damage Falloff", "Decimal. Vanilla is 1. Higher values mean less piercing damage falloff, so more damage overall.");
            ProcCoefficient = ConfigOption(1.5f, "Proc Coefficient", "Vanilla is 1.5");
            Radius = ConfigOption(2f, "Radius", "Vanilla is 2");
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
                self.bulletRadius = Radius;
            }
            orig(self);
        }
    }
}