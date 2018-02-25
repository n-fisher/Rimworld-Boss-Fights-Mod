using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    class BossFightUtility
    {
        private static readonly List<LifeStageAge> defLifeStages = new List<LifeStageAge>() {
            /*new LifeStageAge() {
                def = BossFightDefOf.AnimalBaby,
                minAge = 0,
            },
            new LifeStageAge() {
                def = BossFightDefOf.AnimalJuvenile,
                minAge = 0.01f,
            },*/
            new LifeStageAge() {
                def = BossFightDefOf.AnimalAdult,
                minAge = 0,
                soundAngry = BossFightDefOf.Angry,
                soundCall = BossFightDefOf.Call,
                soundDeath = BossFightDefOf.Death,
                soundWounded = BossFightDefOf.Pain
            },
        };
        private static PawnKindDef[] enabledBossTypes = { PawnKindDefOf.Thrumbo };

        public const float VanillaBossSizeMultiplier = 5;
        public const float VanillaBossPowerMultiplier = 2;
        public const float VanillaBossSpeedMultiplier = .67f;
        public const float VanillaBossCooldownMultiplier = 1 / VanillaBossSpeedMultiplier;
        public const int VanillaBossMinimumAge = 1000;

        private static List<PawnKindLifeStage> PawnKindLifeStages(PawnKindDef kind)
        {
            //get last, adult size
            PawnKindLifeStage ret = kind.lifeStages.Last();
            //make it even bigger
            ret.bodyGraphicData.drawSize *= VanillaBossSizeMultiplier;
            //set the dessicated graphic to the largest available
            ret.dessicatedBodyGraphicData.texPath = "Things/Pawn/Animal/Dessicated/CritterDessicatedMedium";
            //make it bigger
            ret.dessicatedBodyGraphicData.drawSize *= VanillaBossSizeMultiplier;
            return new List<PawnKindLifeStage>() { ret };
        }

        public static Pawn GenerateAnimal(PawnKindDef animal, int tile)
        {
            return PawnGenerator.GeneratePawn(new PawnGenerationRequest(animal, null, PawnGenerationContext.NonPlayer, tile));
        }

        private static void CreateBossifiedVanillaAnimalDefs(PawnKindDef animal)
        {
            ThingDef def = BossFightDefOf.ThingDefs.First(thing => thing.defName == animal.label);
            ThingDef bossDef = BossifyVanillaAnimalDef(def);
            DefDatabase<ThingDef>.Add(bossDef);
            BossFightDefOf.BossDefs.Add(bossDef.label, bossDef);

            PawnKindDef bossKind = BossifyVanillaAnimalKind(animal);
            DefDatabase<PawnKindDef>.Add(bossKind);
            BossFightDefOf.BossKinds.Add(bossKind.label, bossKind);
        }

        public static void BossifyVanillaAnimals(bool all=false)
        {
            IEnumerable animals = all ?
                DefDatabase<PawnKindDef>.AllDefs.Where(def => def.RaceProps.Animal) : 
                enabledBossTypes;

            foreach (PawnKindDef animal in animals)
            {
                CreateBossifiedVanillaAnimalDefs(animal);
            }
        }

        public static ThingDef BossifyVanillaAnimalDef(ThingDef animal)
        {
            ThingDef ret = animal;

            //Rename def for saving
            ret.defName = "Boss" + ret.defName;
            ret.label = "Boss " + ret.label;

            //Modify StatBaseValues
            float newMoveSpeed = ret.GetStatValueAbstract(StatDefOf.MoveSpeed) * VanillaBossSpeedMultiplier;
            ret.SetStatBaseValue(StatDefOf.MoveSpeed, newMoveSpeed);

            //TODO: Come up with a better market value multiplier
            float newMarketValue = ret.GetStatValueAbstract(StatDefOf.MarketValue) * VanillaBossPowerMultiplier;
            ret.SetStatBaseValue(StatDefOf.MoveSpeed, newMoveSpeed);

            float newMeatAmount = ret.GetStatValueAbstract(StatDefOf.MeatAmount) * VanillaBossSizeMultiplier;
            ret.SetStatBaseValue(StatDefOf.MoveSpeed, newMoveSpeed);

            //Modify defs
            RaceProperties race = ret.race;
            race.manhunterOnDamageChance = 1;
            race.manhunterOnTameFailChance = 1;
            race.herdAnimal = false;
            race.herdMigrationAllowed = false;
            race.wildness = 1;
            race.lifeExpectancy = short.MaxValue;
            race.lifeStageAges = defLifeStages;
            race.soundMeleeHitPawn = BossFightDefOf.BossHitPawnSound;
            race.soundMeleeHitBuilding = BossFightDefOf.BossHitBuildingSound;
            race.soundMeleeMiss = BossFightDefOf.BossMissSound;
            race.baseBodySize *= VanillaBossSizeMultiplier;
            race.baseHealthScale *= VanillaBossPowerMultiplier;

            ret.tools.ForEach(tool => {
                tool.power *= VanillaBossPowerMultiplier;
                tool.cooldownTime *= VanillaBossCooldownMultiplier;
            });

            return ret;
        }

        public static PawnKindDef BossifyVanillaAnimalKind(PawnKindDef animal)
        {
            PawnKindDef ret = animal;
            ret.defName = "Boss" + ret.defName;
            ret.label = "Boss " + ret.label;
            BossFightDefOf.BossDefs.TryGetValue(ret.label, out ret.race);
            ret.combatPower *= VanillaBossPowerMultiplier;
            ret.canArriveManhunter = true;
            ret.wildSpawn_spawnWild = false;
            ret.lifeStages = PawnKindLifeStages(animal);
            ret.minGenerationAge = VanillaBossMinimumAge;
            return ret;
        }
    }
}