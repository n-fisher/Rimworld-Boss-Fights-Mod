using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    class BossFightDefGenerator
    {
        private static bool generatorHasRan = false;

        public static void BossifyVanillaAnimals(bool all = false)
        {
            IEnumerable<PawnKindDef> animals = all ?
                DefDatabase<PawnKindDef>.AllDefs.Where(def => def.RaceProps.Animal) :
                BossFightSettings.enabledBossTypes;
            Log.Message("animal count:" + animals.Count());
            if (!generatorHasRan)
            {
                if (animals.Any()) {
                    foreach (PawnKindDef animal in animals) {
                        CreateBossifiedVanillaAnimalDefs(animal);
                    }
                    generatorHasRan = true;
                }
            }
        }

        private static void CreateBossifiedVanillaAnimalDefs(PawnKindDef animal)
        {
            //build animal's ThingDef
            ThingDef def = ThingDef.Named(animal.defName);
            ThingDef bossDef = BossifyVanillaAnimalDef(def);
            DefDatabase<ThingDef>.Add(bossDef);
            BossFightDefOf.BossDefs.Add(bossDef.defName, bossDef);

            //build PawnKindDef
            PawnKindDef bossKind = BossifyVanillaAnimalKind(animal);
            DefDatabase<PawnKindDef>.Add(bossKind);
            BossFightDefOf.BossKinds.Add(bossKind.defName, bossKind);
        }

        public static ThingDef BossifyVanillaAnimalDef(ThingDef animal)
        {
            ThingDef ret = animal;

            //Rename def for saving
            ret.defName = "Boss" + ret.defName;
            ret.label = "Boss " + ret.label;

            //Modify StatBaseValues
            float newMoveSpeed = ret.GetStatValueAbstract(StatDefOf.MoveSpeed) * BossFightSettings.VanillaBossSpeedMultiplier;
            ret.SetStatBaseValue(StatDefOf.MoveSpeed, newMoveSpeed);

            //TODO: Come up with a better market value multiplier
            float newMarketValue = ret.GetStatValueAbstract(StatDefOf.MarketValue) * BossFightSettings.VanillaBossPowerMultiplier;
            ret.SetStatBaseValue(StatDefOf.MoveSpeed, newMoveSpeed);

            float newMeatAmount = ret.GetStatValueAbstract(StatDefOf.MeatAmount) * BossFightSettings.VanillaBossSizeMultiplier;
            ret.SetStatBaseValue(StatDefOf.MoveSpeed, newMoveSpeed);

            //Modify defs
            RaceProperties race = ret.race;
            race.manhunterOnDamageChance = 1;
            race.manhunterOnTameFailChance = 1;
            race.herdAnimal = false;
            race.herdMigrationAllowed = false;
            race.wildness = 1;
            race.lifeExpectancy = short.MaxValue;
            race.lifeStageAges = BossFightDefOf.defLifeStages;
            race.soundMeleeHitPawn = BossFightDefOf.BossHitPawnSound;
            race.soundMeleeHitBuilding = BossFightDefOf.BossHitBuildingSound;
            race.soundMeleeMiss = BossFightDefOf.BossMissSound;
            race.baseBodySize *= BossFightSettings.VanillaBossSizeMultiplier;
            race.baseHealthScale *= BossFightSettings.VanillaBossPowerMultiplier;
            ret.race = race;

            ret.tools.ForEach(tool => {
                tool.power *= BossFightSettings.VanillaBossPowerMultiplier;
                tool.cooldownTime *= BossFightSettings.VanillaBossCooldownMultiplier;
            });

            return ret;
        }

        public static PawnKindDef BossifyVanillaAnimalKind(PawnKindDef animal)
        {
            PawnKindDef ret = animal;
            ret.defName = "Boss" + ret.defName;
            ret.label = "Boss " + ret.label;
            BossFightDefOf.BossDefs.TryGetValue(ret.label, out ret.race);
            ret.combatPower *= BossFightSettings.VanillaBossPowerMultiplier;
            ret.canArriveManhunter = true;
            ret.wildSpawn_spawnWild = false;
            ret.lifeStages = PawnKindLifeStages(animal);
            ret.minGenerationAge = BossFightSettings.VanillaBossMinimumAge;
            ret.race = BossFightDefOf.BossDefs.TryGetValue(ret.defName);
            return ret;
        }

        private static List<PawnKindLifeStage> PawnKindLifeStages(PawnKindDef kind)
        {
            //get last, adult size
            PawnKindLifeStage ret = kind.lifeStages.Last();
            //make it even bigger
            ret.bodyGraphicData.drawSize *= BossFightSettings.VanillaBossSizeMultiplier;
            //set the dessicated graphic to the largest available
            ret.dessicatedBodyGraphicData.texPath = "Things/Pawn/Animal/Dessicated/CritterDessicatedMedium";
            //make it bigger
            ret.dessicatedBodyGraphicData.drawSize *= BossFightSettings.VanillaBossSizeMultiplier;
            return kind.lifeStages;
        }
    }
}
