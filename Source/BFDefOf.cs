using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;

namespace Boss_Fight_Mod
{
    [StaticConstructorOnStartup]
    class BossFightDefOf
    {
        public static readonly LifeStageDef AnimalBaby = new BossLifeStageDef("baby");
        public static readonly LifeStageDef AnimalJuvenile = new BossLifeStageDef("juvenile");
        public static readonly LifeStageDef AnimalAdult = new BossLifeStageDef("adult");

        //variables to hold "magic numbers" found in defs
        private static readonly FloatRange zeroToSeventy = new FloatRange(0, 70);
        public static readonly float OneOneThree = 1.137647f;
        private static readonly float DotNineFive = 0.9541176f;
        private static readonly float DotNineThree = 0.9311765f;
        public static readonly SoundDef BossHitPawnSound = new BossActionSoundDef
        {
            defName = "BossSoundHitPawn",
            subSounds = {
                new BossActionSubSoundDef {
                    name = "BossSubSoundHit_Pawn",
                    parentDef = BossHitPawnSound,
                    grains = new List<AudioGrain>() { new BossAudioGrain_Folder("Pawn/Animal/Melee_Big/Hit_Pawn") },
                }
            }
        };
        public static readonly SoundDef BossHitBuildingSound = new BossActionSoundDef
        {
            defName = "BossSoundHitPawn",
            subSounds = {
                new BossActionSubSoundDef {
                    parentDef = BossHitBuildingSound,
                    name = "BossSubSoundHit_Building",
                    grains = new List<AudioGrain>() { new BossAudioGrain_Folder("Pawn/Animal/Melee_Big/Hit_Building") },
                }
            }
        };
        public static SoundDef BossMissSound = new BossActionSoundDef
        {
            defName = "BossSoundMiss",
            subSounds = {
                new BossActionSubSoundDef {
                    parentDef = BossMissSound,
                    name = "BossSubSoundMiss",
                    grains = new List<AudioGrain>() { new BossAudioGrain_Folder("Pawn/Animal/Melee_Big/Miss") },
                    pitchRange = new FloatRange(DotNineThree, 1.045882f)
                }
            }
        };
        public static readonly SoundDef Call = new BossSoundDef
        {
            defName = "BossSoundCall",
            subSounds = {
                new BossSubSoundDef {
                    parentDef = Call,
                    name = "BossSubSoundCall",
                    grains = new List<AudioGrain>() { new BossAudioGrain_Folder("Pawn/Animal/Thrumbo/Thrumbo_Call") },
                    volumeRange = new FloatRange(18, 18),
                    distRange = new FloatRange(0, 50.40025f),
                }
            }
        };
        public static readonly SoundDef Angry = new BossSoundDef
        {
            defName = "BossSoundAngry",
            maxVoices = 2,
            subSounds = {
                new BossSubSoundDef {
                    parentDef = Angry,
                    name = "BossSubSoundAngry",
                    grains = new List<AudioGrain>() { new BossAudioGrain_Folder("Pawn/Animal/Thrumbo/Thrumbo_Angry") },
                    volumeRange = new FloatRange(25, 25),
                    pitchRange = new FloatRange(DotNineFive, OneOneThree)
                }
            }
        };
        public static readonly SoundDef Pain = new BossSoundDef
        {
            defName = "BossSoundPain",
            subSounds = {
                new BossSubSoundDef {
                    parentDef = Pain,
                    name = "BossSubSoundPain",
                    grains = new List<AudioGrain>() { new BossAudioGrain_Folder("Pawn/Animal/Thrumbo/Thrumbo_Pain") },
                    volumeRange = new FloatRange(17, 17),
                    pitchRange = new FloatRange(DotNineFive, 1.091765f),
                    distRange = zeroToSeventy,
                    repeatMode = RepeatSelectMode.NeverTwice
                }
            }
        };
        public static readonly SoundDef Death = new BossSoundDef
        {
            defName = "BossSoundDeath",
            subSounds = {
                new BossSubSoundDef {
                    parentDef = Death,
                    name = "BossSubSoundDeath",
                    grains = new List<AudioGrain>() { new BossAudioGrain_Folder("Pawn/Animal/Thrumbo/Thrumbo_Death") },
                    volumeRange = new FloatRange(30, 30),
                    pitchRange = new FloatRange(DotNineThree, 1.068824f),
                    distRange = zeroToSeventy
                }
            }
        };
        public static readonly List<LifeStageAge> defLifeStages = new List<LifeStageAge>() {
            new LifeStageAge {
                def = AnimalBaby,
                minAge = 0
            },
            new LifeStageAge {
                def = AnimalJuvenile,
                minAge = 0.01f
            },
            new LifeStageAge {
                def = AnimalAdult,
                minAge = 0.02f,
                soundAngry = Angry,
                soundCall = Call,
                soundDeath = Death,
                soundWounded = Pain
            },
        };
        public static FactionDef BossFaction = new FactionDef
        {
            autoFlee = false,
            defName = "BFBossDef",
            canMakeRandomly = false,
            earliestRaidDays = 60,
            goodwillDailyFall = 100,
            hidden = true,
            humanlikeFaction = false,
            isPlayer = false,
            label = "boss",
            maxCountAtGameStart = 1,
            mustStartOneEnemy = true,
            naturalColonyGoodwill = new FloatRange(-100, -100),
            pawnsPlural = "bosses",
            requiredCountAtGameStart = 1,
            startingGoodwill = new FloatRange(-100, -100),
            techLevel = TechLevel.Transcendent,
            canUseAvoidGrid = true,
            fixedName = "Boss"
        };


        public static List<PawnKindDef> BossKinds = new List<PawnKindDef>();
        public static List<ThingDef> BossDefs = new List<ThingDef>();
        public static List<SoundDef> BossSounds = new List<SoundDef> { BossHitPawnSound, BossHitBuildingSound, BossMissSound, Pain, Angry, Death, Call };
    }
}
