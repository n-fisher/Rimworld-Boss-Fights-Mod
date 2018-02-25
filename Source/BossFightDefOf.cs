using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    class BossFightDefOf
    {
        private static IEnumerable<SoundDef> allSoundDefs;
        private static SoundDef bossHitPawnSound;
        private static SoundDef bossHitBuildingSound;
        private static SoundDef bossMissSound;
        private static SoundDef call;
        private static SoundDef angry;
        private static SoundDef pain;
        private static SoundDef death;
        //private static readonly LifeStageDef animalBaby = DefDatabase<LifeStageDef>.GetNamed("AnimalBaby");
        //private static readonly LifeStageDef animalJuvenile = DefDatabase<LifeStageDef>.GetNamed("AnimalJuvenile");
        private static LifeStageDef animalAdult;
        private static IEnumerable<ThingDef> thingDefs = DefDatabase<ThingDef>.AllDefs;

        public static IEnumerable<SoundDef> AllSoundDefs => allSoundDefs ?? SetAllSoundDefs();
        public static IEnumerable<ThingDef> ThingDefs => thingDefs ?? SetAllThingDefs();
        public static SoundDef BossMissSound => bossMissSound ?? SetBossMissSound();
        public static SoundDef BossHitBuildingSound => bossHitBuildingSound ?? SetBossHitBuildingSound();
        public static SoundDef BossHitPawnSound => bossHitPawnSound ?? SetBossHitPawnSound();
        public static SoundDef Call => call ?? SetCall();
        public static SoundDef Angry => angry ?? SetAngry();
        public static SoundDef Pain => pain ?? SetPain();
        public static SoundDef Death => death ?? SetDeath();
        //public static LifeStageDef AnimalBaby => animalBaby;
        //public static LifeStageDef AnimalJuvenile => animalBaby;
        public static LifeStageDef AnimalAdult => animalAdult ?? SetLifeStage();
        public static Dictionary<string, PawnKindDef> BossKinds = new Dictionary<string, PawnKindDef>();
        public static Dictionary<string, ThingDef> BossDefs = new Dictionary<string, ThingDef>();

        private static IEnumerable<ThingDef> SetAllThingDefs() => thingDefs = DefDatabase<ThingDef>.AllDefs;
        private static IEnumerable<SoundDef> SetAllSoundDefs() => allSoundDefs = DefDatabase<SoundDef>.AllDefs;
        private static SoundDef SetBossMissSound() => bossMissSound = AllSoundDefs.Where(def => def.defName == "Pawn_Melee_BigBash_Miss").FirstOrFallback(null);
        private static SoundDef SetBossHitBuildingSound() => bossHitBuildingSound = AllSoundDefs.Where(def => def.defName == "Pawn_Melee_BigBash_HitBuilding").FirstOrFallback(null);
        private static SoundDef SetBossHitPawnSound() => bossHitPawnSound = AllSoundDefs.Where(def => def.defName == "Pawn_Melee_BigBash_HitPawn").FirstOrFallback(null);
        private static SoundDef SetCall() => call = allSoundDefs.Where(def => def.defName == "Pawn_Thrumbo_Call").FirstOrFallback(null);
        private static SoundDef SetAngry() => angry = allSoundDefs.Where(def => def.defName == "Pawn_Thrumbo_Angry").FirstOrFallback(null);
        private static SoundDef SetPain() => pain = allSoundDefs.Where(def => def.defName == "Pawn_Thrumbo_Pain").FirstOrFallback(null);
        private static SoundDef SetDeath() => death = allSoundDefs.Where(def => def.defName == "Pawn_Thrumbo_Death").FirstOrFallback(null);
        private static LifeStageDef SetLifeStage() => animalAdult = DefDatabase<LifeStageDef>.AllDefs.Where(def => def.defName == "AnimalAdult").FirstOrFallback(null);
    }
}
