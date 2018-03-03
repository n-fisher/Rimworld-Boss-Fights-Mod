﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Boss_Fight_Mod
{
    public class IncidentWorker_BossFight : IncidentWorker_ManhunterPack
    {
        private Lord bossLord;
        private Faction faction;

        protected void GuardNullStatics(IncidentParms parms)
        {
            if (BossFightDefOf.AllowedBossKinds == null) {
                BossFightDefOf.AllowedBossKinds = DefDatabase<PawnKindDef>.AllDefs.Where(def =>
                    def.RaceProps?.Animal ?? false && CombatPowerCalculator.BodyMoveCoverages.Keys.Contains(def.RaceProps?.body?.defName)
                ).ToList();
            }
            if (BossFightDefOf.AllowedBossDefs == null) {
                BossFightDefOf.AllowedBossDefs = new List<ThingDef>(DefDatabase<ThingDef>.AllDefs.Where(def =>
                    def.race?.Animal ?? false && CombatPowerCalculator.BodyMoveCoverages.Keys.Contains(def.race?.body?.defName))
                );
            }
            if (faction == null) {
                faction = FactionGenerator.NewGeneratedFaction(BossFightDefOf.BossFaction);
                Find.FactionManager.Add(faction);
                Find.VisibleMap.pawnDestinationReservationManager.RegisterFaction(faction);
            }

            if (bossLord == null) {
                bossLord = LordMaker.MakeNewLord(faction, new LordJob_BossAssault(faction), (Map) parms.target);
                Find.VisibleMap.lordManager.AddLord(bossLord);
            }
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map) parms.target;

            GuardNullStatics(parms);

            if (!RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 intVec, map, CellFinder.EdgeRoadChance_Animal, null)) {
                return false;
            }
            Pawn boss = BossFightUtility.GenerateAnimal(map.Tile, faction, parms.points);
            Rot4 rot = Rot4.FromAngleFlat((map.Center - intVec).AngleFlat);
            bossLord.AddPawn(boss);

            // so many asthmatic bosses
            boss.health.Reset();
            GenSpawn.Spawn(boss, intVec, map, rot, false);

            Find.LetterStack.ReceiveLetter("Boss Fight", "The birds go silent and the ground trembles below you… BOSS FIGHT INCOMING", LetterDefOf.ThreatBig, boss, null);
            return true;
        }
    }
}