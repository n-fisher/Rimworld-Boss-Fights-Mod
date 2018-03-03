using System;
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

            return SpawnNewPawn(def, t, f);
        }

        private static Pawn SpawnNewPawn(PawnKindDef animal, int tile, Faction faction)
        {
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(animal, faction, PawnGenerationContext.NonPlayer, tile));
            pawn.thingIDNumber = Find.UniqueIDsManager.GetNextThingID();
            return pawn;
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