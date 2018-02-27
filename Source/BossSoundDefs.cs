using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Boss_Fight_Mod
{

    public class BossSoundDef : SoundDef
    {
        public BossSoundDef(string path) : base()
        {
            defName = "BossSound" + path.Substring(path.LastIndexOf("/") + 1);
            context = SoundContext.MapOnly;
        }

        public BossSoundDef() : base()
        {
            context = SoundContext.MapOnly;
        }
    }

    public class BossSubSoundDef : SubSoundDef
    {
        public BossSubSoundDef(string path) : base()
        {
            name = "BossSubSound" + path.Substring(path.LastIndexOf("/") + 1);
            grains = new List<AudioGrain>() { new BossAudioGrain_Folder(path) };
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

        public override IEnumerable<ResolvedGrain> GetResolvedGrains()
        {
            using (IEnumerator<AudioClip> enumerator = ContentFinder<AudioClip>.GetAllInFolder(clipFolderPath).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    yield return new ResolvedGrain_Clip(enumerator.Current);
                }
            }
            yield break;
        }
    }

    public class BossActionSoundDef : BossSoundDef
    {
        public BossActionSoundDef(string action = "") : base(action)
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
