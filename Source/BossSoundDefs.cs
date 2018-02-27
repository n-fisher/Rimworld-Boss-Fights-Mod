using System.Collections.Generic;
using Verse;
using Verse.Sound;

namespace Boss_Fight_Mod
{

    public class BossSoundDef : SoundDef
    {
        public BossSoundDef() : base()
        {
            context = SoundContext.MapOnly;
        }
    }

    public class BossSubSoundDef : SubSoundDef
    {
        public BossSubSoundDef(string path)
        {
            grains = new List<AudioGrain> {
                new AudioGrain_Folder() {
                    clipFolderPath = path
                }
            };
            pitchRange = new FloatRange(0.9770588f, 1.137647f);
            sustainLoop = false;
        }
    }

    public class BossActionSoundDef : BossSoundDef
    {
        public BossActionSoundDef(string action = "") : base()
        {
            maxVoices = 2;
            subSounds = new List<SubSoundDef>() {
                new BossActionSubSoundDef(action)
            };
        }
    }

    public class BossActionSubSoundDef : BossSubSoundDef
    {
        public BossActionSubSoundDef(string action) : base(action)
        {
            volumeRange = new FloatRange(20, 20);
        }
    }
}
