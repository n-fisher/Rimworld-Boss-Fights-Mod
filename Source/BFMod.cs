using System.Linq;
using Verse;

namespace Boss_Fight_Mod
{
    class BossFightMod : Mod
    {
        public BossFightMod(ModContentPack content) : base(content)
        {
            //BossFightDefGenerator.BossifyVanillaAnimals(true);
            BossFightDefOf.BossSounds.ForEach(sound => sound.subSounds[0].parentDef = sound);
            BossFightDefOf.BossKinds = new System.Collections.Generic.List<PawnKindDef>(DefDatabase<PawnKindDef>.AllDefs.Where(def => def.RaceProps.Animal));
        }
    }
}
