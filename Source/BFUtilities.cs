using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    public class BossFightUtility
    {
        public enum BuffCat {
            Damage, Speed, Cooldown, Health, Accuracy, Size
        }

        public static Dictionary<string, IEnumerable<BuffCat>> BuffStrategies = new Dictionary<string, IEnumerable<BuffCat>>();

        public static Pawn GenerateAnimal(int t, Faction f, float points)
        {
            PawnKindDef boss = BossFightDefOf.AllowedBossKinds.RandomElement();
            KeyValuePair<string, IEnumerable<BuffCat>> strat = BuffStrategies.RandomElement();

            Debug.Log("Initializing a " + strat.Key + " " + boss.defName + " with " + points + " points...");

            BossPawnKindDef def = new BossPawnKindDef(boss, points, strat.Value);

            Debug.Log("Created a boss with " + def.combatPower + " power.\nBuffs: " + def.BuffMultiple.ToStringFullContents());

            return GenerateAnimal(def, t, f);
        }

        private static Pawn GenerateAnimal(PawnKindDef animal, int tile, Faction faction)
        {
            return PawnGenerator.GeneratePawn(new PawnGenerationRequest(animal, faction, PawnGenerationContext.NonPlayer, tile));
        }
    }

    public class Debug
    {
        [Conditional("DEBUG")]
        public static void Log(string s)
        {
            Verse.Log.Message(s);
        }
    }
}