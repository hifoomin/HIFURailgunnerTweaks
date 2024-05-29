using R2API;
using RoR2;
using System;
using System.Collections;
using UnityEngine;

namespace HIFURailgunnerTweaks.Misc
{
    public class Hurtboxes : MiscBase
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

            var railgunnerHurtboxUnfucker = self.GetComponent<RailgunnerHurtboxUnfucker>() ? self.GetComponent<RailgunnerHurtboxUnfucker>() : self.gameObject.AddComponent<RailgunnerHurtboxUnfucker>();
            if (!railgunnerHurtboxUnfucker.initialized)
            {
                self.StartCoroutine(UnfuckHurtboxes(self));

                railgunnerHurtboxUnfucker.initialized = true;
            }
        }

        public IEnumerator UnfuckHurtboxes(HurtBox self)
        {
            yield return new WaitForSeconds(0.03f);
            if (self.isSniperTarget)
            {
                if (self.hurtBoxGroup)
                {
                    var newHurtBox = GameObject.Instantiate(self);
                    newHurtBox.transform.parent = self.transform.parent;
                    newHurtBox.transform.localScale *= Size;

                    HurtBox.sniperTargetsList.Add(newHurtBox);
                    newHurtBox.isInSniperTargetList = true;

                    if (newHurtBox.GetComponent<RailgunnerHurtboxUnfucker>() == null)
                    {
                        var railgunnerHurtboxUnfucker = newHurtBox.gameObject.AddComponent<RailgunnerHurtboxUnfucker>();
                        railgunnerHurtboxUnfucker.initialized = true;
                    }

                    self.isSniperTarget = false;

                    Array.Resize(ref self.hurtBoxGroup.hurtBoxes, self.hurtBoxGroup.hurtBoxes.Length + 1);
                    self.hurtBoxGroup.hurtBoxes[self.hurtBoxGroup.hurtBoxes.Length - 1] = self;
                }
                else // for no hurtboxgroup mfs just in case
                {
                    var newHurtBox = GameObject.Instantiate(self);
                    newHurtBox.transform.parent = self.transform.parent;
                    newHurtBox.transform.localScale *= Size;

                    HurtBox.sniperTargetsList.Add(newHurtBox);
                    newHurtBox.isInSniperTargetList = true;

                    if (newHurtBox.GetComponent<RailgunnerHurtboxUnfucker>() == null)
                    {
                        var railgunnerHurtboxUnfucker = newHurtBox.gameObject.AddComponent<RailgunnerHurtboxUnfucker>();
                        railgunnerHurtboxUnfucker.initialized = true;
                    }

                    self.isSniperTarget = false;
                }

                self.isInSniperTargetList = false;
                HurtBox.sniperTargetsList.Remove(self);
            }

            yield return new WaitForSeconds(0.01f);
        }

        public class RailgunnerHurtboxUnfucker : MonoBehaviour
        {
            public bool initialized = false;
        }
    }
}