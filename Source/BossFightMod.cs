using System.Reflection;
using Harmony;
using UnityEngine;
using Verse;

namespace Boss_Fight_Mod
{
    class BossFightMod : Mod
    {
        public BossFightMod(ModContentPack content) : base(content)
        {
            HarmonyInstance.Create("com.fyarn.boss_fight_mod").PatchAll(Assembly.GetExecutingAssembly());
            BossFightDefGenerator.BossifyVanillaAnimals(true);
            BossFightDefOf.BossSounds.ForEach(sound => {
                Log.Message(sound.subSounds[0].parentDef.ToStringSafe());
                sound.subSounds[0].parentDef = sound;
                Log.Message(sound.subSounds[0].parentDef.ToStringSafe());

            });
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
