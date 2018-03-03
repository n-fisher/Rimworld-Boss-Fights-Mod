using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;
using static Boss_Fight_Mod.BossFightUtility;

namespace Boss_Fight_Mod
{
    //might have to fix copying debug id's/short hashes in constructors
    public class BossPawnKindDef : PawnKindDef
    {
        private Dictionary<BuffCat, float> buffMultiple;

        public Dictionary<BuffCat, float> BuffMultiple => buffMultiple;

        public BossPawnKindDef(PawnKindDef def, float points, IEnumerable<BuffCat> strategy)
        {
            foreach (FieldInfo field in def.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)) {
                field.SetValue(this, field.GetValue(def));
            }

            defName = "Boss" + def.defName + points;
            labelFemale = labelMale = label = "Boss " + def.label;
            combatPower = CombatPowerCalculator.BuffUpToThreshold(def, points, strategy, out buffMultiple);
            canArriveManhunter = true;
            wildSpawn_spawnWild = false;
            lifeStages = BossFightDefOf.PawnKindLifeStages(def.lifeStages, buffMultiple[BuffCat.Size]);
            minGenerationAge = BossFightSettings.VanillaBossMinimumAge;
            race = new BossPawnThingDef(BossFightDefOf.AllowedBossDefs.First(thingDef => thingDef.defName == def.defName), points, buffMultiple);

            ResolveReferences();
        }
    }

    public class BossPawnThingDef : ThingDef
    {
        public BossPawnThingDef(ThingDef def, float points, Dictionary<BuffCat, float> buffMultipliers)
        {
            // Define if bugs are reported
            //public List<StatModifier> equippedStatOffsets;
            //startingHpRange
            //public List<CompProperties> comps;
            foreach (FieldInfo field in def.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)) {
                field.SetValue(this, field.GetValue(def));
            }
            
            defName = "Boss" + def.defName + points;
            label = "Boss " + label;

            statBases = new List<StatModifier>();
            size = new IntVec2(
                def.size.x * (int) Math.Max((buffMultipliers[BuffCat.Size] * BossFightSettings.SizeScalar), 1),
                def.size.z * (int) Math.Max((buffMultipliers[BuffCat.Size] * BossFightSettings.SizeScalar), 1)
            );

            BossifyTools(def, buffMultipliers);
            BossifyStats(def, buffMultipliers);
            BossifyRace(def, buffMultipliers);

            ResolveReferences();
        }

        private void BossifyRace(ThingDef def, Dictionary<BuffCat, float> buffMultipliers)
        {
            //define if bugs reported
            //public PawnNameCategory nameCategory;
            //public List<HediffGiverSetDef> hediffGiverSets;

            //define in future features
            //public List<AnimalBiomeRecord> wildBiomes;
            //public SimpleCurve ageGenerationCurve;
            //public bool makesFootprints;
            //

            race = new RaceProperties();
            foreach (FieldInfo field in race.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)) {
                field.SetValue(race, field.GetValue(def.race));
            }
            race.manhunterOnDamageChance = 1;
            race.manhunterOnTameFailChance = 1;
            race.herdAnimal = false;
            race.herdMigrationAllowed = false;
            race.wildness = 1;
            race.lifeExpectancy = short.MaxValue;
            race.lifeStageAges = new List<LifeStageAge>(BossFightDefOf.defLifeStages);
            race.soundMeleeHitPawn = BossFightDefOf.BossHitPawnSound;
            race.soundMeleeHitBuilding = BossFightDefOf.BossHitBuildingSound;
            race.soundMeleeMiss = BossFightDefOf.BossMissSound;
            race.baseBodySize *= buffMultipliers[BuffCat.Size];
            race.baseHealthScale *= buffMultipliers[BuffCat.Health];
            race.needsRest = false;

            race.body = def.race.body;

            race.predator = true;
            race.maxPreyBodySize = float.MaxValue;
            race.canBePredatorPrey = false;
        }

        private void BossifyStats(ThingDef def, Dictionary<BuffCat, float> buffMultipliers)
        {
            this.SetStatBaseValue(StatDefOf.MoveSpeed,
                def.GetStatValueAbstract(StatDefOf.MoveSpeed) * buffMultipliers[BuffCat.Speed]);

            this.SetStatBaseValue(StatDefOf.MarketValue,
                def.GetStatValueAbstract(StatDefOf.MarketValue) * buffMultipliers.Values.Sum());

            this.SetStatBaseValue(StatDefOf.MeatAmount,
                def.GetStatValueAbstract(StatDefOf.MeatAmount) * buffMultipliers[BuffCat.Size]);

            this.SetStatBaseValue(StatDefOf.Mass,
                def.GetStatValueAbstract(StatDefOf.Mass) * buffMultipliers[BuffCat.Size]);

            this.SetStatBaseValue(StatDefOf.LeatherAmount,
                def.GetStatValueAbstract(StatDefOf.LeatherAmount) * buffMultipliers[BuffCat.Size]);
        }

        private void BossifyTools(ThingDef def, Dictionary<BuffCat, float> buffMultipliers)
        {
            tools = new List<Tool>();
            def.tools.ForEach(tool => {
                BossTool bossTool = new BossTool(tool);
                bossTool.power *= buffMultipliers[BuffCat.Damage];
                bossTool.cooldownTime *= buffMultipliers[BuffCat.Cooldown];
                tools.Add(bossTool);
            });
        }
    }


    /*public class BossBodyDef : BodyDef
    {

        public BossBodyDef(BodyDef def, Dictionary<BuffCat, float> buffMultipliers)
        {
            foreach (FieldInfo field in def.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                field.SetValue(this, field.GetValue(def));
            }
            defName = "Boss" + def.defName;
        }
    }
    public class BossCorePart : BodyPartRecord
    {
        public BossCorePart(BodyPartRecord corePart, Dictionary<BuffCat, float> buffMultipliers)
        {
            foreach (FieldInfo field in corePart.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                field.SetValue(this, field.GetValue(corePart));
            }
            def = corePart.def;
            parent = corePart.parent;
            groups = corePart.groups;
            depth = corePart.depth;
            height = corePart.height;
            parts = corePart.parts;
        }
    }
    */
    public class BossTool : Tool
    {
        public BossTool(Tool tool)
        {
            foreach (FieldInfo field in tool.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)) {
                field.SetValue(this, field.GetValue(tool));
            }
            id = "Boss" + tool.id;

            /*capacities = new List<ToolCapacityDef>(tool.capacities);

            surpriseAttack = new SurpriseAttackProps();
            surpriseAttack.extraMeleeDamages = new List<ExtraMeleeDamage>(tool.surpriseAttack.extraMeleeDamages);

            linkedBodyPartsGroup = new BodyPartGroupDef();
            linkedBodyPartsGroup.listOrder = tool.linkedBodyPartsGroup.listOrder;*/

        }
    }
}