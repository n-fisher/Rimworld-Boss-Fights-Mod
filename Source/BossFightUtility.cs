using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    class BossFightUtility
    {
        public static Pawn GenerateAnimal(PawnKindDef animal, int tile)
        {
            PawnGenerationRequest x = new PawnGenerationRequest(animal, null, PawnGenerationContext.NonPlayer, tile);
            return PawnGenerator.GeneratePawn(x);
        }
    }
}