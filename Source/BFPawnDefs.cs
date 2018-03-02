using System.Reflection;
using Verse;

namespace Boss_Fight_Mod
{
    public class BossPawnKindDef : PawnKindDef
    {
        public BossPawnKindDef(PawnKindDef def)
        {
            foreach (FieldInfo field in def.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                field.SetValue(this, field.GetValue(def));
            }
        }
    }

    public class BossPawnThingDef : ThingDef
    {
        public BossPawnThingDef(ThingDef def)
        {
            foreach (FieldInfo field in def.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                field.SetValue(this, field.GetValue(def));
            }
        }
    }
}