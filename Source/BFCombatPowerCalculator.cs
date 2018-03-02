using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    public static class CombatPowerCalculator
    {
        //public so other mods may add to this list
        public static Dictionary<string, float> BodyMoveCoverages = new Dictionary<string, float>
        {
            new KeyValuePair<String, float>("Bird", 0.547f),
            new KeyValuePair<String, float>("BeetleLike", 0.476f),
            new KeyValuePair<String, float>("BeetleLikeWithClaw", 0.476f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithPaws", 0.578f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithPawsAndTail", 0.57f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithHooves", 0.578f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithHoovesAndHump", 0.5382f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithHoovesAndTusks", 0.5502f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithHoovesTusksAndTrunk", 0.541f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithHoovesAndHorn", 0.5612f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithClawsTailAndJowl", 0.56f),
            new KeyValuePair<String, float>("TurtleLike", 0.441f),
            new KeyValuePair<String, float>("Monkey", 0.4571f),
            new KeyValuePair<String, float>("Snake", 0.78f),
        };

        public static Dictionary<string, float> BodyVitalCoverages = new Dictionary<string, float>
        {
            new KeyValuePair<String, float>("Bird", 0.558f),
            new KeyValuePair<String, float>("BeetleLike", 0.461f),
            new KeyValuePair<String, float>("BeetleLikeWithClaw", 0.459f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithPaws", 0.579f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithPawsAndTail", 0.519f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithHooves", 0.579f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithHoovesAndHump", 0.519f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithHoovesAndTusks", 0.53f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithHoovesTusksAndTrunk", 0.514f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithHoovesAndHorn", 0.543f),
            new KeyValuePair<String, float>("QuadrupedAnimalWithClawsTailAndJowl", 0.532f),
            new KeyValuePair<String, float>("TurtleLike", 0.55f),
            new KeyValuePair<String, float>("Monkey", 0.4231f),
            new KeyValuePair<String, float>("Snake", 0.87f),
        };


        const float BaseHumanMoveSpeed = 4.61f;
        private readonly static SimpleCurve curve = new SimpleCurve {
            new CurvePoint(10, 20),
            new CurvePoint(100, 100),
            new CurvePoint(175, 167.5f),
            new CurvePoint(300, 255),
            new CurvePoint(500, 355),
            new CurvePoint(99999, 20255)
        };

        public static int CombatPower(ThingDef def)
        {
            return (int)FinalCombatPower(def);
        } 

        public static float MoveScore(ThingDef def)
        {
            return MoveScore(
                def.statBases.First(modifier => modifier.stat == StatDefOf.MoveSpeed).value,
                BodyMoveCoverage(def.race.body)
            );
        }

        //private static List<string> movementTags = new List<string>() { "MovingLimbCore", "MovingLimbSegment", "MovingLimbDigit" };

        private static float FinalCombatPower(ThingDef def)
        {
            return curve.Evaluate(BaseCombatPower(def));
        }

        private static float BaseCombatPower(ThingDef def)
        {
            return 17.5f * DPS(def) * HealthScore(def) * MoveScore(def);
        }

        public static float HealthScore(ThingDef def)
        {
            return def.race.baseHealthScale 
                / (1 + (RangeAccuracyMultiplier(def.race.baseBodySize) - 1) / 2)
                / ((Multiplier(def, StatDefOf.ArmorRating_Blunt) + Multiplier(def, StatDefOf.ArmorRating_Sharp)) / 2)
                / ((BodyVitalCoverage(def.race.body) + 0.5f) / 2)
                / 2;
        }

        private static float Multiplier(ThingDef def, StatDef statDef)
        {
            float armor = def.statBases.First(modifier => modifier.stat == statDef).value;
            return (1 - DamageReduction(armor)) * (1 - Deflection(armor));
        }

        private static float Deflection(float armor)
        {
            return Math.Min(0.9f, armor < 0.5 ? 0 : (
                    armor < 1 ? armor - 0.5f : 0.5f + (armor - 1) / 4
                )
            );
        }

        private static float DamageReduction(float armor)
        {
            return Math.Min(0.9f, armor < 0.5 ? armor : (
                    armor < 1 ? 0.5f : 0.5f + (armor - 1) / 4
                )
            );
        }

        public static float DPS(ThingDef def)
        {
            return 0.62f * SumToolDPSContribs(def.tools);
        }

        private static float SumToolDPSContribs(List<Tool> tools)
        {
            float ret = 0;
            float totalCommonality = 0;

            foreach (Tool t in tools) {
                totalCommonality += t.commonality;
                ret += t.power / t.cooldownTime * t.commonality;
            }
            ret /= totalCommonality;
            return ret;
        }

        private static float BodyMoveCoverage(BodyDef bodyDef)
        {
            return BodyMoveCoverages.GetValueSafe(bodyDef.defName);
        }

        private static float BodyVitalCoverage(BodyDef bodyDef)
        {
            return BodyVitalCoverages.GetValueSafe(bodyDef.defName);
        }

        private static float MoveScore(float moveSpeed, float bodyMovePercent) {
            return moveSpeed / bodyMovePercent / BaseHumanMoveSpeed;
        }

        private static float RangeAccuracyMultiplier(float bodySize) {
            return Math.Max(Math.Min(0.5f, bodySize), bodySize);
        }
    }
}
