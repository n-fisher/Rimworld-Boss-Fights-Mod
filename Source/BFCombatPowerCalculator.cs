using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using RimWorld;
using Verse;
using static Boss_Fight_Mod.BossFightUtility;

namespace Boss_Fight_Mod
{
    public static class CombatPowerCalculator
    {
        private const float buffIncrement = 0.15f;
        //public so other mods may add their own bosses to this list
        public static Dictionary<string, float> BodyMoveCoverages = new Dictionary<string, float>();
        public static Dictionary<string, float> BodyVitalCoverages = new Dictionary<string, float>();


        const float BaseHumanMoveSpeed = 4.61f;
        private readonly static SimpleCurve curve = new SimpleCurve {
            new CurvePoint(10, 20),
            new CurvePoint(100, 100),
            new CurvePoint(175, 167.5f),
            new CurvePoint(300, 255),
            new CurvePoint(500, 355),
            new CurvePoint(99999, 20255)
        };

        public static float BuffUpToThreshold(PawnKindDef def, float points, IEnumerable<BuffCat> strategy, out Dictionary<BuffCat, float> buffMultipliers, int MaxBuffAttempts = 10000)
        {
            Dictionary<BuffCat, float> buffs = new Dictionary<BuffCat, float> {
                [BuffCat.Accuracy] = 1,
                [BuffCat.Cooldown] = 1 * BossFightSettings.CDScalar,
                [BuffCat.Damage] = 1,
                [BuffCat.Health] = 1 * BossFightSettings.HealthScalar,
                [BuffCat.Size] = 1,
                [BuffCat.Speed] = 1
            };

            Debug.Log(buffs.ToStringFullContents());
            float power = def.combatPower;
            float powerAfterBuff;
            int buffAttempts;

            // TODO: Make a binary search fn instead of linear to zoom
            for (buffAttempts = 0; buffAttempts < MaxBuffAttempts; buffAttempts++) {
                BuffCat buff = strategy.RandomElement();
                Debug.Log(power + ": " + buffs.ToStringSafeEnumerable());
                powerAfterBuff = PowerIfBuffed(def, buffs, buff);
                if (powerAfterBuff < points) {
                    power = powerAfterBuff;
                    if (buff == BuffCat.Cooldown) {
                        // reduce by 15% every time without hitting 0.
                        buffs[buff] *= (1 - buffIncrement);
                    } else {
                        buffs[buff] += buffIncrement;
                    }
                } else {
                    break;
                }
            }
            if (buffAttempts >= MaxBuffAttempts) {
                Log.Warning("Took too long generating a powerful enough boss. Buff attempts capped at " + MaxBuffAttempts);
            }
            buffMultipliers = buffs;
            Debug.Log("Took " + buffAttempts + " buffs to create a " + power + " Boss " + def.label);
            return power;
        }

        private static float PowerIfBuffed(PawnKindDef def, Dictionary<BuffCat, float> buffMultipliers, BuffCat buff)
        {
            Dictionary<BuffCat, float> buffs = new Dictionary<BuffCat, float>(buffMultipliers);
            if (buff == BuffCat.Cooldown) {
                // reduce by 15% every time without hitting 0.
                buffs[buff] *= (1 - buffIncrement);
            } else {
                buffs[buff] += buffIncrement;
            }
            return CombatPower(def.race, buffs);
        }

        public static int CombatPower(ThingDef def) => (int) FinalCombatPower(def);

        private static int CombatPower(ThingDef def, Dictionary<BuffCat, float> buffMultipliers) => (int) FinalCombatPower(def, buffMultipliers);

        private static float FinalCombatPower(ThingDef def, Dictionary<BuffCat, float> buffMultipliers)
        {
            return curve.Evaluate(BaseCombatPower(def, buffMultipliers));
        }

        private static float BaseCombatPower(ThingDef def, Dictionary<BuffCat, float> buffMultipliers)
        {
            return 17.5f * DPS(def, buffMultipliers) * HealthScore(def, buffMultipliers) * MoveScore(def, buffMultipliers);
        }

        private static float HealthScore(ThingDef def, Dictionary<BuffCat, float> buffMultipliers)
        {
            return def.race.baseHealthScale * buffMultipliers[BuffCat.Health]
                / (1 + (RangeAccuracyMultiplier(def.race.baseBodySize * buffMultipliers[BuffCat.Size]) - 1) / 2)
                / ((Multiplier(def.statBases?.FirstOrDefault(modifier =>
                    modifier.stat == StatDefOf.ArmorRating_Blunt)?.value ?? 0) +
                    Multiplier(def.statBases?.FirstOrDefault(modifier =>
                        modifier.stat == StatDefOf.ArmorRating_Sharp)?.value ?? 0)) / 2)
                / ((BodyVitalCoverage(def.race.body) + 0.5f) / 2)
                / 2;
        }

        private static float DPS(ThingDef def, Dictionary<BuffCat, float> buffMultipliers) => 0.62f * SumToolDPSContribs(def.tools, buffMultipliers);

        private static float SumToolDPSContribs(List<Tool> tools, Dictionary<BuffCat, float> buffMultipliers)
        {
            float ret = 0;
            float totalCommonality = 0;

            float powerBuff = buffMultipliers[BuffCat.Damage];
            float cdBuff = buffMultipliers[BuffCat.Cooldown];

            foreach (Tool t in tools) {
                totalCommonality += t.commonality;
                ret += (t.power * powerBuff) / (cdBuff * t.cooldownTime) * t.commonality;
            }

            return ret / totalCommonality;
        }

        private static float MoveScore(ThingDef def, Dictionary<BuffCat, float> buffMultipliers)
        {
            return MoveScore(
                def.statBases.First(modifier => modifier.stat == StatDefOf.MoveSpeed).value * buffMultipliers[BuffCat.Speed],
                BodyMoveCoverage(def.race.body)
            );
        }

        public static float MoveScore(ThingDef def)
        {
            return MoveScore(
                def.statBases.First(modifier => modifier.stat == StatDefOf.MoveSpeed).value,
                BodyMoveCoverage(def.race.body)
            );
        }

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
            return Multiplier(armor);
        }

        private static float Multiplier(float armor)
        {
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

        private static float MoveScore(float moveSpeed, float bodyMovePercent)
        {
            return moveSpeed / bodyMovePercent / BaseHumanMoveSpeed;
        }

        private static float RangeAccuracyMultiplier(float bodySize)
        {
            return Math.Min(Math.Max(0.5f, bodySize), 2);
        }
    }
}
