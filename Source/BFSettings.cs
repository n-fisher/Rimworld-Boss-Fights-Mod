using System.Collections.Generic;
using RimWorld;
using Verse;
using static Boss_Fight_Mod.BossFightUtility;

namespace Boss_Fight_Mod
{
    public class BossFightSettings : ModSettings
    {
        public static readonly List<PawnKindDef> enabledBossTypes = new List<PawnKindDef> { PawnKindDef.Named("Monkey") };
        public const int VanillaBossMinimumAge = 1000;
        //public const int MaxBuffAttempts = 100;

        public const float SizeFinalScalar = 0.5f;
        public const float SizeMultMax = 6;

        public const float CooldownInitialScalar = 2.0f;
        public const float CooldownMin = 0.15f;

        public static readonly Dictionary<BuffCat, float> BuffIncrements = new Dictionary<BuffCat, float> {
            //[BuffCat.Size] = 0.20f,
            [BuffCat.Cooldown] = 0.05f,
            //[BuffCat.Accuracy] = 0.15f,
            [BuffCat.Damage] = 0.20f,
            [BuffCat.Health] = 0.25f,
            [BuffCat.Speed] = 0.10f
        };
        public const float PointConversionFactor = 2f;

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
