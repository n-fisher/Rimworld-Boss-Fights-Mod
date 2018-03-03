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

        public static float BuffUpToThreshold(PawnKindDef def, float points, IEnumerable<BuffCat> strategyWeights, out Dictionary<BuffCat, float> buffMultipliers, int MaxBuffAttempts = 10000)
        {
            List<BuffCat> strategy = new List<BuffCat>(strategyWeights);
            Dictionary<BuffCat, float> buffs = new Dictionary<BuffCat, float> {
                [BuffCat.Accuracy] = 1,
                [BuffCat.Cooldown] = 1 * BossFightSettings.CooldownInitialScalar,
                [BuffCat.Damage] = 1,
                [BuffCat.Health] = 1,
                [BuffCat.Size] = 1,
                [BuffCat.Speed] = 1
            };

            float power = def.combatPower;
            float powerAfterBuff;
            int buffAttempts;
            List<BuffCat> maxedBuffs = new List<BuffCat>();

            // TODO: Make a binary search fn instead of linear to zoom
            for (buffAttempts = 0; buffAttempts < MaxBuffAttempts; buffAttempts++) {
                if (strategy.NullOrEmpty()) {
                    break;
                }
                BuffCat buff = strategy.RandomElement();
                Debug.Log(power + ": " + buffs.ToStringSafeEnumerable());
                powerAfterBuff = PowerIfBuffed(def, buffs, buff);
                //Debug.Log("Chosen " + buff.ToString() + "Buff would increase power to " + powerAfterBuff + "," + (powerAfterBuff < points ? " applying buff..." : " breaking generation."));

                if (powerAfterBuff == power || powerAfterBuff >= points) {
                    Debug.Log("Buffing " + buff + " is too powerful, removing...");
                    strategy.RemoveAll(buffCat => buffCat == buff);
                    continue;
                } else {
                    power = powerAfterBuff;
                    IncrementBuff(ref buffs, buff);
                }
            }
            if (buffAttempts >= MaxBuffAttempts) {
                Log.Warning("Took too long generating a powerful enough boss. Buff attempts capped at " + MaxBuffAttempts);
            }
            buffMultipliers = buffs;
            Debug.Log("Took " + buffAttempts + " buffs to create a " + power + " Boss " + def.label);
            return power;
        }

        private static bool IncrementBuff(ref Dictionary<BuffCat, float> buffs, BuffCat buff)
        {
            switch (buff) {
                // reduce by 15% every time without hitting 0.
                case BuffCat.Cooldown:
                    if (buffs[buff] * (1 - BossFightSettings.BuffIncrements[buff]) <= BossFightSettings.CooldownMin) {
                        return false;
                    }
                    break;
                // limit to a max size
                case BuffCat.Size:
                    if (buffs[buff] + BossFightSettings.BuffIncrements[buff] >= BossFightSettings.SizeMax) {
                        return false;
                    }
                    break;
            }

            Buff(ref buffs, buff);
            return true;
        }

        private static void Buff(ref Dictionary<BuffCat, float> buffs, BuffCat buff)
        {
            buffs[buff] = (buff == BuffCat.Cooldown) ?
                buffs[buff] * (1 - BossFightSettings.BuffIncrements[buff]) :
                buffs[buff] + BossFightSettings.BuffIncrements[buff];
        }

        private static float PowerIfBuffed(PawnKindDef def, Dictionary<BuffCat, float> buffMultipliers, BuffCat buff)
        {
            Dictionary<BuffCat, float> buffs = new Dictionary<BuffCat, float>(buffMultipliers);
            if (!IncrementBuff(ref buffs, buff)) {
                Log.Warning("Your puny colony couldn't stand up to a regular " + def.label + ", much less a boss.");
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
