using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    class BossFightUtility
    {
        public static Pawn GenerateAnimal(PawnKindDef animal, int tile, Faction faction)
        {
            PawnGenerationRequest x = new PawnGenerationRequest(animal, faction, PawnGenerationContext.NonPlayer, tile);
            return PawnGenerator.GeneratePawn(x);
        }
    }
}