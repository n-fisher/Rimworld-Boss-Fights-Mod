using Harmony;
using Verse;

namespace Boss_Fight_Mod
{/*
    [HarmonyPatch(typeof(SoundDefHelper), "CorrectContextNow")]
    class SoundDefHelperPatch
    {
        [HarmonyPrefix]
        public static void Prefix(SoundDef def, Map sourceMap)
        {
            if (def == null)
                Log.Message(def.ToStringSafe() + "\n" + sourceMap.ToStringSafe());
        }
    }*/
    [HarmonyPatch(typeof(LifeStageUtility), "GetNearestLifestageSound")]
    class  Patch
    {
        [HarmonyPrefix]
        public static void Postfix(ref SoundDef def)
        {
                Log.Message("def " + def.ToStringSafe());
        }
    }
}