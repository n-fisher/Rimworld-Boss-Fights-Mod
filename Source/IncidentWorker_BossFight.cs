using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    public class IncidentWorker_BossFight : IncidentWorker_ManhunterPack
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            BossFightDefGenerator.BossifyVanillaAnimals(true);
            Map map = (Map) parms.target;
            
            if (!RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 intVec, map, CellFinder.EdgeRoadChance_Animal, null)) {
                return false;
            }
            PawnKindDef def = BossFightDefOf.BossKinds.RandomElement();
            Pawn boss = BossFightUtility.GenerateAnimal(def, map.Tile);
            Rot4 rot = Rot4.FromAngleFlat((map.Center - intVec).AngleFlat);
            GenSpawn.Spawn(boss, intVec, map, rot, false);
            boss.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, true, false, null);
            Find.LetterStack.ReceiveLetter("Boss Fight", "The birds go silent and the ground trembles below you… BOSS FIGHT INCOMING", LetterDefOf.ThreatBig, boss, null);
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }
    }
}