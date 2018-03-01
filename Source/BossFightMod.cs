using UnityEngine;
using Verse;

namespace Boss_Fight_Mod
{
    class BossFightMod : Mod
    {
        public BossFightMod(ModContentPack content) : base(content)
        {
            BossFightDefGenerator.BossifyVanillaAnimals(true);
            BossFightDefOf.BossSounds.ForEach(sound => sound.subSounds[0].parentDef = sound);
        }
    }
}
