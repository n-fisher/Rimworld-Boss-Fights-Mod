using RimWorld;

namespace Boss_Fight_Mod
{
    class BossLifeStageDef : LifeStageDef
    {
        public BossLifeStageDef(string label="adult")
        {
            this.label = label;
            visible = false;
            reproductive = false;
            milkable = false;
            shearable = false;
        }
    }
}
