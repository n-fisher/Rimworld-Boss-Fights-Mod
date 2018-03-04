using System.Collections.Generic;
using System.Linq;
using Verse;
using static Boss_Fight_Mod.BossFightUtility;

namespace Boss_Fight_Mod
{
    class BossFightMod : Mod
    {
        public BossFightMod(ModContentPack content) : base(content)
        {
            BossFightDefOf.BossSounds.ForEach(sound => sound.subSounds[0].parentDef = sound);
            InitCalculatorLists();
            Debug.Log("BodyMoveCoverages:" + CombatPowerCalculator.BodyMoveCoverages.Count);
            Debug.Log("BodyVitalCoverages:" + CombatPowerCalculator.BodyVitalCoverages.Count);
            InitBossStrategies();
            Debug.Log(BuffStrategies.ToStringSafeEnumerable());
        }

        private void InitCalculatorLists()
        {
            CombatPowerCalculator.BodyMoveCoverages["Bird"] = 0.547f;
            CombatPowerCalculator.BodyMoveCoverages.Add("BeetleLike", 0.476f);
            CombatPowerCalculator.BodyMoveCoverages.Add("BeetleLikeWithClaw", 0.476f);
            CombatPowerCalculator.BodyMoveCoverages.Add("QuadrupedAnimalWithPaws", 0.578f);
            CombatPowerCalculator.BodyMoveCoverages.Add("QuadrupedAnimalWithPawsAndTail", 0.57f);
            CombatPowerCalculator.BodyMoveCoverages.Add("QuadrupedAnimalWithHooves", 0.578f);
            CombatPowerCalculator.BodyMoveCoverages.Add("QuadrupedAnimalWithHoovesAndHump", 0.5382f);
            CombatPowerCalculator.BodyMoveCoverages.Add("QuadrupedAnimalWithHoovesAndTusks", 0.5502f);
            CombatPowerCalculator.BodyMoveCoverages.Add("QuadrupedAnimalWithHoovesTusksAndTrunk", 0.541f);
            CombatPowerCalculator.BodyMoveCoverages.Add("QuadrupedAnimalWithHoovesAndHorn", 0.5612f);
            CombatPowerCalculator.BodyMoveCoverages.Add("QuadrupedAnimalWithClawsTailAndJowl", 0.56f);
            CombatPowerCalculator.BodyMoveCoverages.Add("TurtleLike", 0.441f);
            CombatPowerCalculator.BodyMoveCoverages.Add("Monkey", 0.4571f);
            CombatPowerCalculator.BodyMoveCoverages.Add("Snake", 0.78f);

            CombatPowerCalculator.BodyVitalCoverages.Add("Bird", 0.558f);
            CombatPowerCalculator.BodyVitalCoverages.Add("BeetleLike", 0.461f);
            CombatPowerCalculator.BodyVitalCoverages.Add("BeetleLikeWithClaw", 0.459f);
            CombatPowerCalculator.BodyVitalCoverages.Add("QuadrupedAnimalWithPaws", 0.579f);
            CombatPowerCalculator.BodyVitalCoverages.Add("QuadrupedAnimalWithPawsAndTail", 0.519f);
            CombatPowerCalculator.BodyVitalCoverages.Add("QuadrupedAnimalWithHooves", 0.579f);
            CombatPowerCalculator.BodyVitalCoverages.Add("QuadrupedAnimalWithHoovesAndHump", 0.519f);
            CombatPowerCalculator.BodyVitalCoverages.Add("QuadrupedAnimalWithHoovesAndTusks", 0.53f);
            CombatPowerCalculator.BodyVitalCoverages.Add("QuadrupedAnimalWithHoovesTusksAndTrunk", 0.514f);
            CombatPowerCalculator.BodyVitalCoverages.Add("QuadrupedAnimalWithHoovesAndHorn", 0.543f);
            CombatPowerCalculator.BodyVitalCoverages.Add("QuadrupedAnimalWithClawsTailAndJowl", 0.532f);
            CombatPowerCalculator.BodyVitalCoverages.Add("TurtleLike", 0.55f);
            CombatPowerCalculator.BodyVitalCoverages.Add("Monkey", 0.4231f);
            CombatPowerCalculator.BodyVitalCoverages.Add("Snake", 0.87f);
        }

        private void InitBossStrategies()
        {
            // Add three instances of each cat to insulate extremes from custom boss generator styles
            BuffStrategies.Add("Flat",
                new List<BuffCat> {
                    BuffCat.Damage, BuffCat.Damage, BuffCat.Damage,
                    BuffCat.Speed, BuffCat.Speed, BuffCat.Speed,
                    BuffCat.Cooldown, BuffCat.Cooldown, BuffCat.Cooldown,
                    BuffCat.Health, BuffCat.Health, BuffCat.Health,
                    //BuffCat.Accuracy, BuffCat.Accuracy, BuffCat.Accuracy,
                    //BuffCat.Size, BuffCat.Size, BuffCat.Size,
                });
            BuffStrategies.Add("Avatarfighter_Brute", Buff(
                new List<BuffCat> { BuffCat.Speed },
                new List<BuffCat> { }));
            BuffStrategies.Add("Avatarfighter_Basher", Buff(
                new List<BuffCat> { BuffCat.Damage, BuffCat.Speed },
                new List<BuffCat> { BuffCat.Health }));
            BuffStrategies.Add("Avatarfighter_Rogue", Buff(
                new List<BuffCat> { BuffCat.Speed, BuffCat.Damage },
                new List<BuffCat> { }));
            BuffStrategies.Add("Avatarfigher_OnyxTip", Buff(
                new List<BuffCat> { BuffCat.Damage, BuffCat.Damage, BuffCat.Damage, BuffCat.Health, /*BuffCat.Size, BuffCat.Size, BuffCat.Size */},
                new List<BuffCat> { BuffCat.Speed, BuffCat.Speed }));
            BuffStrategies.Add("fyarn_PlayGroundBully", Buff(
                new List<BuffCat> { BuffCat.Damage, BuffCat.Damage, BuffCat.Speed, BuffCat.Cooldown, BuffCat.Cooldown, /* BuffCat.Size, BuffCat.Size */},
                new List<BuffCat> { BuffCat.Health }));
            BuffStrategies.Add("fyarn_PowerPixie", Buff(
                new List<BuffCat> { },
                new List<BuffCat> { BuffCat.Health, BuffCat.Health, BuffCat.Health, BuffCat.Size, BuffCat.Size, BuffCat.Size }));
        }

        private IEnumerable<BuffCat> Buff(IEnumerable<BuffCat> buffs, IEnumerable<BuffCat> nerfs)
        {
            List<BuffCat> ret = buffs.ToList();
            ret.AddRange(BuffStrategies["Flat"]);
            foreach (BuffCat nerf in nerfs) {
                ret.Remove(nerf);
            }
            return ret;
        }
    }
}
