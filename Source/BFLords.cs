using RimWorld;
using Verse;
using Verse.AI;
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

    public class LordToil_BossAssault : LordToil_AssaultColony
    {
        public override void UpdateAllDuties()
        {
            foreach (Pawn p in lord.ownedPawns) {
                p.mindState.duty = new BossFightDuty();
            }
        }
    }

    public class BossFightDuty : PawnDuty
    {
        //TODO: Create custom jobgivers to allow more boss-like behavior
        // JobGiver_AITrashBuildingsDistant
        public BossFightDuty()
        {
            locomotion = LocomotionUrgency.Sprint;
            canDig = true;
            def = new DutyDef {
                defName = "BossAssaultColony",
                alwaysShowWeapon = true,
                thinkNode = new ThinkNode_Priority {
                    subNodes = {
                        //unnecessary for now, need to reenable with adding human bosses
                        //new JobGiver_TakeCombatEnhancingDrug(),

                        new JobGiver_BossFightEnemy(),
                        new JobGiver_AITrashColonyClose(),
                        new JobGiver_AITrashBuildingsDistant(),
                        new JobGiver_AIGotoNearestHostile(),
                        new JobGiver_AITrashBuildingsDistant(),
                        new JobGiver_AISapper()
                    }
                }
            };
        }
    }

    public class JobGiver_BossFightEnemy : JobGiver_AIFightEnemy
    {
        const float TargetAcquireRadius = 1000;
        const float TargetKeepRadius = 50;
        const int TicksSinceEngageToLoseTarget = 200;
        const int ExpiryTicks = 60;
        /*
        public override float GetPriority(Pawn pawn)
        {
            return base.GetPriority(pawn);
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
        {
            return base.TryIssueJobPackage(pawn, jobParams);
        }

        protected override void ResolveSubnodes()
        {
            base.ResolveSubnodes();
        }*/

        protected override Job TryGiveJob(Pawn pawn)
        {
            UpdateEnemyTarget(pawn);
            Thing target = pawn.mindState.enemyTarget;
            if (target == null) {
                return null;
            }

            Verb verb = pawn.TryGetAttackVerb();
            if (verb == null) {
                return null;
            } else if (verb.verbProps.MeleeRange) {
                return MeleeAttackJob(target);
            }

            if (((CoverUtility.CalculateOverallBlockChance(pawn.Position, target.Position, pawn.Map) > 0.01f && pawn.Position.Standable(pawn.Map))
                    || (pawn.Position - target.Position).LengthHorizontalSquared < 25)
                && verb.CanHitTarget(target)) {
                return new Job(JobDefOf.WaitCombat, ExpiryTicks, true);
            }

            if (!TryFindShootingPosition(pawn, out IntVec3 intVec)) {
                return null;
            }

            if (intVec == pawn.Position) {
                return new Job(JobDefOf.WaitCombat, ExpiryTicks, true);
            }

            Job job = new Job(JobDefOf.Goto, intVec);
            job.expiryInterval = ExpiryTicks;
            job.checkOverrideOnExpire = true;
            return job;
        }

        protected override Job MeleeAttackJob(Thing enemyTarget)
        {
            Job job = new Job(JobDefOf.AttackMelee, enemyTarget);
            job.expiryInterval = ExpiryTicks;
            job.checkOverrideOnExpire = true;
            job.expireRequiresEnemiesNearby = true;
            return job;
        }

        //TODO: Populate when ready for armed bosses
        protected override bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest)
        {
            Log.Warning("Trying to find a shooting position for a boss. This shouldn't happen in this version.");
            dest = default(IntVec3);
            return false;
        }

        protected override void UpdateEnemyTarget(Pawn pawn)
        {
            Thing target = pawn.mindState.enemyTarget;
            if (target != null && 
                (target.Destroyed || 
                Find.TickManager.TicksGame - pawn.mindState.lastEngageTargetTick > 400 || 
                !pawn.CanReach(target, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn) || 
                (pawn.Position - target.Position).LengthHorizontalSquared > TargetKeepRadius * TargetKeepRadius ||
                ((IAttackTarget) target).ThreatDisabled())) {
                target = null;
            }

            if (target == null) {
                target = FindAttackTargetIfPossible(pawn);
                if (target != null) {
                    pawn.mindState.lastEngageTargetTick = Find.TickManager.TicksGame;
                    pawn.GetLord()?.Notify_PawnAcquiredTarget(pawn, target);
                }
            } else {
                Thing newTarget = FindAttackTargetIfPossible(pawn);

                if (newTarget != null && newTarget != target) {
                    pawn.mindState.lastEngageTargetTick = Find.TickManager.TicksGame;
                }
                target = newTarget;
            }

            pawn.mindState.enemyTarget = target;
            if (target is Pawn colonist && 
                colonist.Faction == Faction.OfPlayer && 
                pawn.Position.InHorDistOf(colonist.Position, TargetAcquireRadius)) {
                Find.TickManager.slower.SignalForceNormalSpeed();
            }
        }

		private Thing FindAttackTargetIfPossible(Pawn pawn)
		{
			return pawn.TryGetAttackVerb() == null ?
				null :
                FindAttackTarget(pawn);
		}

		protected override Thing FindAttackTarget(Pawn pawn)
		{
			TargetScanFlags targetScanFlags = TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedReachableIfCantHitFromMyPos | TargetScanFlags.NeedThreat;
			/*if (PrimaryVerbIsIncendiary(pawn))
			{
				targetScanFlags |= TargetScanFlags.NeedNonBurning;
			}*/
			return (Thing)AttackTargetFinder.BestAttackTarget(pawn, targetScanFlags, 
                x => ExtraTargetValidator(pawn, x), 0f, TargetAcquireRadius, 
                default(IntVec3), float.MaxValue, false);
		}

		/*private bool PrimaryVerbIsIncendiary(Pawn pawn)
		{
			if (pawn.equipment != null && pawn.equipment.Primary != null)
			{
				List<Verb> allVerbs = pawn.equipment.Primary.GetComp<CompEquippable>().AllVerbs;
				for (int i = 0; i < allVerbs.Count; i++)
				{
					if (allVerbs[i].verbProps.isPrimary)
					{
						return allVerbs[i].IsIncendiary();
					}
				}
			}
			return false;
		}*/
    }
}