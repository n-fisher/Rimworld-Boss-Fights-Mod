using System.Collections.Generic;
using RimWorld;
using Verse;

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
        public static readonly SoundDef BossHitPawnSound = new BossActionSoundDef("Pawn/Animal/Melee_Big/Hit_Pawn");
        public static readonly SoundDef BossHitBuildingSound = new BossActionSoundDef("Pawn/Animal/Melee_Big/Hit_Building");

        public static SoundDef BossMissSound = new BossActionSoundDef("Pawn/Animal/Melee_Big/Miss") {
            subSounds = {
                new BossActionSubSoundDef("Pawn/Animal/Melee_Big/Miss") {
                    pitchRange = new FloatRange(DotNineThree, 1.045882f)
                }
            }
        };
        public static readonly SoundDef Call = new BossSoundDef("Pawn/Animal/Thrumbo/Thrumbo_Call") {
            subSounds = {
                new BossSubSoundDef("Pawn/Animal/Thrumbo/Thrumbo_Call") {
                    volumeRange = new FloatRange(18, 18),
                    distRange = new FloatRange(0, 50.40025f),
                }
            }
        };
        public static readonly SoundDef Angry = new BossSoundDef("Pawn/Animal/Thrumbo/Thrumbo_Angry") {
            maxVoices = 2,
            subSounds = {
                new BossSubSoundDef("Pawn/Animal/Thrumbo/Thrumbo_Angry") {
                    volumeRange = new FloatRange(25, 25),
                    pitchRange = new FloatRange(DotNineFive, OneOneThree)
                }
            }
        };
        public static readonly SoundDef Pain = new BossSoundDef("Pawn/Animal/Thrumbo/Thrumbo_Pain") {
            subSounds = {
                new BossSubSoundDef("Pawn/Animal/Thrumbo/Thrumbo_Pain") {
                    volumeRange = new FloatRange(17, 17),
                    pitchRange = new FloatRange(DotNineFive, 1.091765f),
                    distRange = zeroToSeventy,
                    repeatMode = Verse.Sound.RepeatSelectMode.NeverTwice
                }
            }
        };
        public static readonly SoundDef Death = new BossSoundDef("Pawn/Animal/Thrumbo/Thrumbo_Death") {
            subSounds = {
                new BossSubSoundDef("Pawn/Animal/Thrumbo/Thrumbo_Death") {
                    volumeRange = new FloatRange(30, 30),
                    pitchRange = new FloatRange(DotNineThree, 1.068824f),
                    distRange = zeroToSeventy
                }
            }
        };
        public static readonly List<LifeStageAge> defLifeStages = new List<LifeStageAge>() {
            new LifeStageAge() {
                def = AnimalBaby,
                minAge = 0
            },
            new LifeStageAge() {
                def = AnimalJuvenile,
                minAge = 0.01f
            },
            new LifeStageAge() {
                def = AnimalAdult,
                minAge = 0.02f,
                soundAngry = Angry,
                soundCall = Call,
                soundDeath = Death,
                soundWounded = Pain
            },
        };
        public static List<PawnKindDef> BossKinds = new List<PawnKindDef>();
        public static List<ThingDef> BossDefs = new List<ThingDef>();
    }
}
