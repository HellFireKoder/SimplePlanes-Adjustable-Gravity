using UnityEngine;
using SPModSettings;

public class GravityManager : MonoBehaviour
{
    Vector3 GravityVector = new Vector3(0.0f, -9.81f, 0.0f); // default gravity
    IModSettingsPage SettingsPage;
    IModSetting GravitySetting;
    IModSetting ActiveSetting;
    IModSetting GravityModeSetting;
    bool Active = true;
    bool LastActive = true;
    GravityMode Mode = GravityMode.Global;
    private enum GravityMode
    {
        Global = 0,
        Relative = 1
    }
    // Update is called once per frame
    void Update()
    {
        if (Active)
        {
            switch (Mode)
            {
                case GravityMode.Global:
                    Physics.gravity = GravityVector; // set the global gravity to our gravity
                    break;
                case GravityMode.Relative:
                    Physics.gravity = Quaternion.Euler(ServiceProvider.Instance.PlayerAircraft.MainCockpitRotation) * GravityVector; // set the global gravity to our gravity, relative to the main cockpit
                    break;
            }
        }
        else if (LastActive)
        {   // make sure gravity is normal when this mod is disabled
            Physics.gravity = new Vector3(0.0f, -9.81f, 0.0f);
        }
        LastActive = Active;
        if (!ModSettingsHandler.ModSettingsReady)
        {   // if the mod settings aren't ready to generate settings, wait
            return;
        }
        if (SettingsPage == null || SettingsPage.Equals(null))
        {   // if we have no settings page
            SettingsPage = ModSettingsHandler.AddSettingsPage("AdjustableGravity", "Adjustable Gravity", "Setting for the global gravity"); // generate page
            ActiveSetting = ModSettingsHandler.AddSettingToPage(SettingsPage, "Active", "Mod Active", ModSettingTypes.Toggle, true.ToString());// and settings
            GravitySetting = ModSettingsHandler.AddSettingToPage(SettingsPage, "Gravity", "Gravity", ModSettingTypes.Vector3, new Vector3(0.0f, -9.81f, 0.0f).ToStringCompact()); 
            GravityModeSetting = ModSettingsHandler.AddSettingToPage(SettingsPage, "GravityMode", "Gravity Mode", ModSettingTypes.Dropdown, "0", GravityMode.Global.ToString(), GravityMode.Relative.ToString());
            SettingsPage.LoadSettings(); // load saved settings (if they've been saved before, otherwise defaults)
            bool.TryParse(ActiveSetting.Value, out Active); // parse active
            ModSettingConverter.Vector3TryParse(GravitySetting.Value, ref GravityVector); // parse the GravitySetting out to our GravityVector
            int GMode = 0;
            int.TryParse(GravityModeSetting.Value, out GMode);
            Mode = (GravityMode)GMode;
        }
    }
}
