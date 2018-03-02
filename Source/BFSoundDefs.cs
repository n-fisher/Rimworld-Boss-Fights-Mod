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
        public BossSubSoundDef() : base()
        {
            pitchRange = new FloatRange(0.9770588f, BossFightDefOf.OneOneThree);
            sustainLoop = false;
            ResolveReferences();
        }
    }

    public class BossAudioGrain_Folder : AudioGrain_Folder
    {
        public BossAudioGrain_Folder(string path) : base()
        {
            clipFolderPath = path;
        }
    }

    public class BossActionSoundDef : BossSoundDef
    {
        public BossActionSoundDef() : base()
        {
            maxVoices = 2;
        }
    }

    public class BossActionSubSoundDef : BossSubSoundDef
    {
        public BossActionSubSoundDef() : base()
        {
            volumeRange = new FloatRange(20, 20);
        }
    }
}
