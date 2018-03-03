using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    public class BossFightSettings : ModSettings
    {
        public static readonly List<PawnKindDef> enabledBossTypes = new List<PawnKindDef> { PawnKindDefOf.Thrumbo };
        public const int VanillaBossMinimumAge = 1000;
        public const int MaxBuffAttempts = 100;
        public const float SizeScalar = 0.33f;
        public const float HealthScalar = 2.0f;
        public const float CDScalar = 2.0f;

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
