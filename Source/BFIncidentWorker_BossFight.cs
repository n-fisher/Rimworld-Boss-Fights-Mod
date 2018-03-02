using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Boss_Fight_Mod
{
    public class IncidentWorker_BossFight : IncidentWorker_ManhunterPack
    {
        private static Lord bossLord;
        private static Faction faction;

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (faction == null) {
                faction = FactionGenerator.NewGeneratedFaction(BossFightDefOf.BossFaction);
                Find.FactionManager.Add(faction);
               Find.VisibleMap.pawnDestinationReservationManager.RegisterFaction(faction);
            }

            Map map = (Map) parms.target;
            if (bossLord == null) {
                bossLord = LordMaker.MakeNewLord(faction, new LordJob_BossAssault(faction), map);
                Find.VisibleMap.lordManager.AddLord(bossLord);
            }

            if (!RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 intVec, map, CellFinder.EdgeRoadChance_Animal, null)) {
                return false;
            }

            ThingDef originalDef = BossFightDefOf.BossDefs.RandomElement();
            ThingDef bossThingDef = BossFightDefGenerator.BossifyVanillaAnimalDef(originalDef);
            PawnKindDef def = BossFightDefGenerator.BossifyVanillaAnimalKind(PawnKindDef.Named(originalDef.defName));

            Pawn boss = BossFightUtility.GenerateAnimal(def, map.Tile, faction);
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