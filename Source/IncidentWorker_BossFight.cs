﻿using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    public class IncidentWorker_BossFight : IncidentWorker_ManhunterPack
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map) parms.target;
            
            if (!RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 intVec, map, CellFinder.EdgeRoadChance_Animal, null)) {
                return false;
            }
            Pawn boss = ManhunterPackIncidentUtility.GenerateAnimals(BossFightDefOf.Thrumbo, map.Tile, 1).FirstOrFallback();
            Rot4 rot = Rot4.FromAngleFlat((map.Center - intVec).AngleFlat);
            IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 10, null);
            GenSpawn.Spawn(boss, loc, map, rot, false);

            boss.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, true, false, null);
            Find.LetterStack.ReceiveLetter("Boss Fight", "The birds go silent and the ground trembles below you… BOSS FIGHT INCOMING", LetterDefOf.ThreatBig, boss, null);
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }
    }
}