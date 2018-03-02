using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    class BossFightUtility
    {
        public static Pawn GenerateAnimal(PawnKindDef animal, int tile, Faction faction)
        {
            return PawnGenerator.GeneratePawn(new PawnGenerationRequest(animal, faction, PawnGenerationContext.NonPlayer, tile));
        }
    }
}