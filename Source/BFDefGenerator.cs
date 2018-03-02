using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    class BossFightDefGenerator
    {
        private static bool generatorHasRan = false;

        public static void BossifyVanillaAnimals(bool all = false)
        {
            if (!generatorHasRan) {
                IEnumerable<PawnKindDef> animals = all ?
                    DefDatabase<PawnKindDef>.AllDefs.Where(def => def.RaceProps.Animal) :
                    BossFightSettings.enabledBossTypes;

                if (animals.Any()) {
                    animals = animals.ToList();
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
            BossFightDefOf.BossDefs.Add(BossifyVanillaAnimalDef(def));

            //build PawnKindDef
            BossFightDefOf.BossKinds.Add(BossifyVanillaAnimalKind(animal));
        }

        //todo move these into bosspawndefs
        public static ThingDef BossifyVanillaAnimalDef(ThingDef animal)
        {
            ThingDef ret = new BossPawnThingDef(animal);

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
            RaceProperties race = new RaceProperties();
            foreach (FieldInfo field in race.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                field.SetValue(race, field.GetValue(ret.race));
            }
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
            string defName = "Boss" + animal.defName;
            PawnKindDef ret = new BossPawnKindDef(animal) {
                defName = defName,
                label = "Boss " + animal.label,
                combatPower = animal.combatPower * BossFightSettings.VanillaBossPowerMultiplier,
                canArriveManhunter = true,
                wildSpawn_spawnWild = false,
                lifeStages = PawnKindLifeStages(animal),
                minGenerationAge = BossFightSettings.VanillaBossMinimumAge,
                race = new BossPawnThingDef(BossFightDefOf.BossDefs.First(def => def.defName == defName)),
            };
            ret.ResolveReferences();
            return ret;
        }

        private static List<PawnKindLifeStage> PawnKindLifeStages(PawnKindDef kind)
        {
            List<PawnKindLifeStage> ret = new List<PawnKindLifeStage>();
            foreach(PawnKindLifeStage stage in kind.lifeStages)
            {
                PawnKindLifeStage pawnKindLifeStage = new PawnKindLifeStage();
                foreach (FieldInfo field in stage.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    field.SetValue(pawnKindLifeStage, field.GetValue(stage));
                }
                //make it even bigger
                pawnKindLifeStage.bodyGraphicData.drawSize *= BossFightSettings.VanillaBossSizeMultiplier;
                //set the dessicated graphic to the largest available
                pawnKindLifeStage.dessicatedBodyGraphicData.texPath = "Things/Pawn/Animal/Dessicated/CritterDessicatedMedium";
                //make it bigger
                pawnKindLifeStage.dessicatedBodyGraphicData.drawSize *= BossFightSettings.VanillaBossSizeMultiplier;
                ret.Add(pawnKindLifeStage);
            }
            return ret;
        }
    }
}
