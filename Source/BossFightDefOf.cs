using Verse;

namespace Boss_Fight_Mod
{
    class BossFightDefOf
    {
        private static PawnKindDef thrumbo = DefDatabase<PawnKindDef>.GetNamed("BossThrumbo");

        public static PawnKindDef Thrumbo { get => thrumbo; }
    }
}
