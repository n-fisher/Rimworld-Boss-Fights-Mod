using RimWorld;
using Verse.AI.Group;

namespace Boss_Fight_Mod
{
    public class LordJob_BossAssault : LordJob_AssaultColony
    {
        public LordJob_BossAssault(Faction assaulterFaction, bool canKidnap = true, bool canTimeoutOrFlee = false, bool sappers = false, bool useAvoidGridSmart = true, bool canSteal = false) : base(assaulterFaction, canKidnap, canTimeoutOrFlee, sappers, useAvoidGridSmart, canSteal)
        {
        }

        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();

            stateGraph.AddToil(new LordToil_AssaultColony {
                avoidGridMode = AvoidGridMode.Smart
            });

            return stateGraph;
        }
    }
}