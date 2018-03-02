using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    class BossFightSettings : ModSettings
    {
        public static readonly List<PawnKindDef> enabledBossTypes = new List<PawnKindDef> { PawnKindDefOf.Thrumbo };
        public static readonly float VanillaBossSizeMultiplier = 5;
        public static readonly float VanillaBossPowerMultiplier = 5;
        public static readonly float VanillaBossSpeedMultiplier = .67f;
        public static readonly float VanillaBossCooldownMultiplier = 1 / VanillaBossSpeedMultiplier;
        public static readonly int VanillaBossMinimumAge = 1000;

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
