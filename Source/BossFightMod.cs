using UnityEngine;
using Verse;

namespace Boss_Fight_Mod
{
    class BossFightMod : Mod
    {
        public BossFightMod(ModContentPack content) : base(content)
        {
            BossFightUtility.BossifyVanillaAnimals();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return base.SettingsCategory();
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
        }
    }
}
