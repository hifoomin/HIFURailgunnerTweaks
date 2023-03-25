using RoR2.Skills;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HRGT.Skills
{
    public class PolarFieldDevice : TweakBase
    {
        public static float SpeedBuffVal;
        public static float SpeedBuffDur;
        public static int Charges;
        public static float Cooldown;
        public static float Radius;
        public static BuffDef SpeedBuff;

        public override string Name => ": Utility :: Polar Field Device";

        public override string SkillToken => "utility_alt";

        public override string DescText => "Throw out a device that <style=cIsUtility>slows down</style> all nearby <style=cIsUtility>enemies and projectiles</style>." +
                                           (SpeedBuffDur > 0 && SpeedBuffVal > 0 ? " <style=cIsUtility>Speeds up</style> allies by <style=cIsUtility>" + d(SpeedBuffVal) + "</style> in its radius." : "") +
                                           (Charges > 1 ? " Can hold up to " + Charges + "." : "");

        public override void Init()
        {
            SpeedBuffVal = ConfigOption(0.3f, "Speed Buff", "Decimal. Vanilla is 0");
            SpeedBuffDur = ConfigOption(2.5f, "Speed Buff Duration", "Vanilla is 0");
            Charges = ConfigOption(1, "Charges", "Vanilla is 1");
            Cooldown = ConfigOption(12f, "Cooldown", "Vanilla is 12");
            Radius = ConfigOption(16f, "Radius", "Vanilla is 10");
            base.Init();
        }

        public override void Hooks()
        {
            MakeBuff();
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            Changes();
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.GetBuffCount(SpeedBuff) > 0)
            {
                args.moveSpeedMultAdd += SpeedBuffVal;
            }
        }

        private void MakeBuff()
        {
            var sprite2d = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texMovespeedBuffIcon.tif").WaitForCompletion();
            SpeedBuff = ScriptableObject.CreateInstance<BuffDef>();
            SpeedBuff.name = "Polar Field Device Speed";
            SpeedBuff.buffColor = new Color32(144, 228, 255, 225);
            SpeedBuff.canStack = false;
            SpeedBuff.iconSprite = Sprite.Create(sprite2d, new Rect(0f, 0f, (float)sprite2d.width, (float)sprite2d.height), new Vector2(0f, 0f));
            SpeedBuff.isDebuff = false;

            ContentAddition.AddBuffDef(SpeedBuff);
        }

        private void Changes()
        {
            var sd = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/Railgunner/RailgunnerBodyFireMineBlinding.asset").WaitForCompletion();
            sd.baseMaxStock = Charges;
            sd.baseRechargeInterval = Cooldown;
            var p = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerMineAltDetonated.prefab").WaitForCompletion();
            var slow = p.GetComponent<BuffWard>();
            slow.radius = Radius;
            var bf = p.AddComponent<BuffWard>();
            bf.buffDuration = SpeedBuffDur;
            bf.buffDef = SpeedBuff;
            bf.radius = Radius;
            bf.expires = true;
            bf.invertTeamFilter = false;
            bf.expireDuration = 10f;
            bf.shape = BuffWard.BuffWardShape.Sphere;
        }
    }
}