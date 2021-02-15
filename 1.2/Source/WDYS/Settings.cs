using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace WDYS
{
    class WDYS_Settings : ModSettings
    {
        public bool onlyIndustrialAndHigher = true;
        public bool needTradeConsole = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.onlyIndustrialAndHigher, "onlyIndustrialAndHigher", true);
            Scribe_Values.Look(ref this.needTradeConsole, "needTradeConsole", true);
        }
    }

    class WDYS_Mod : Mod
    {
        public static WDYS_Settings settings;

        public WDYS_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<WDYS_Settings>();
        }

        public override string SettingsCategory() => "WDYS".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("WDYS.NeedTradeConsole".Translate() + ": ",
                                                ref settings.needTradeConsole);
            if (settings.needTradeConsole)
            {
                listing_Standard.CheckboxLabeled("WDYS.OnlyIndustrial".Translate() + ": ",
                                                ref settings.onlyIndustrialAndHigher);
            }
            listing_Standard.End();
            settings.Write();
        }
    }
}
