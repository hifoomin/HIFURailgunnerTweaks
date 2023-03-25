using R2API;
using RoR2;

namespace HRGT.Misc
{
    public class Hitboxes : MiscBase
    {
        public static float Size;
        public override string Name => ":: Misc :: Weakpoints";

        public override void Init()
        {
            base.Init();
            Size = ConfigOption(0.75f, "Size Percent", "Decimal. Vanilla is 1");
        }

        public override void Hooks()
        {
            On.RoR2.HurtBox.OnEnable += HurtBox_OnEnable;
        }

        private void HurtBox_OnEnable(On.RoR2.HurtBox.orig_OnEnable orig, HurtBox self)
        {
            orig(self);
            HurtBox.sniperTargetRadius = Size;
        }
    }
}