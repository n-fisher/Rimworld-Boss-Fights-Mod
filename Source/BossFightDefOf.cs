using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Boss_Fight_Mod
{
    class BossFightDefOf
    {
        public static readonly LifeStageDef AnimalBaby = new BossLifeStageDef("baby");
        public static readonly LifeStageDef AnimalJuvenile = new BossLifeStageDef("juvenile");
        public static readonly LifeStageDef AnimalAdult = new BossLifeStageDef("adult");
        public static readonly SoundDef BossHitPawnSound = new BossActionSoundDef("Pawn/Animal/Melee_Big/Hit_Pawn");
        public readonly static SoundDef BossHitBuildingSound = new BossActionSoundDef("Pawn/Animal/Melee_Big/Hit_Building");
        public readonly static SoundDef BossMissSound = new BossActionSoundDef() {
            subSounds = {
                new BossActionSubSoundDef("Pawn/Animal/Melee_Big/Miss") {
                    pitchRange = new FloatRange(0.9311765f, 1.045882f)
                }
            }
        };

        public readonly static SoundDef Call = new BossSoundDef() {
            subSounds = {
                new BossSubSoundDef("Pawn/Animal/Thrumbo/Thrumbo_Call") {
                    volumeRange = new FloatRange(18, 18),
                    distRange = new FloatRange(0, 50.40025f),
                }
            }
        };
        public readonly static SoundDef Angry = new BossSoundDef() {
            maxVoices = 2,
            subSounds = {
                new BossSubSoundDef("Pawn/Animal/Thrumbo/Thrumbo_Angry") {
                    volumeRange = new FloatRange(25, 25),
                    pitchRange = new FloatRange(0.9541176f, 1.137647f)
                }
            }
        };
        public readonly static SoundDef Pain = new BossSoundDef() {
            subSounds = {
                new BossSubSoundDef("Pawn/Animal/Thrumbo/Thrumbo_Pain") {
                    volumeRange = new FloatRange(17, 17),
                    pitchRange = new FloatRange(0.9311765f, 1.091765f),
                    distRange = new FloatRange(0, 70),
                    repeatMode = Verse.Sound.RepeatSelectMode.NeverTwice
                }
            }
        };
        public readonly static SoundDef Death = new BossSoundDef() {
            subSounds = {
                new BossSubSoundDef("Pawn/Animal/Thrumbo/Thrumbo_Death") {
                    volumeRange = new FloatRange(30, 30),
                    pitchRange = new FloatRange(0.9311764f, 1.068824f),
                    distRange = new FloatRange(0, 70)
                }
            }
        };
        public static Dictionary<string, PawnKindDef> BossKinds = new Dictionary<string, PawnKindDef>();
        public static Dictionary<string, ThingDef> BossDefs = new Dictionary<string, ThingDef>();
        public static readonly List<LifeStageAge> defLifeStages = new List<LifeStageAge>() {
            new LifeStageAge() {
                def = AnimalBaby,
                minAge = 0,
            },
            new LifeStageAge() {
                def = AnimalJuvenile,
                minAge = 0.01f,
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
    }
}
