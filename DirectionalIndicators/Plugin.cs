using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DirectionalIndicators;

[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
[BepInDependency("JustJelly.AudibleDistanceLib")]
public class Plugin : BaseUnityPlugin
{
    private const string pluginGuid = "notpr.DirectionalIndicators";
    private const string pluginName = "DirectionalIndicators";
    private const string pluginVersion = "0.1.1";

    private Harmony harmony;

    public static Plugin Instance;
    public static ManualLogSource ManualLogSource;

    public DirectionalIndicatorList indicators = [];

    public ConfigEntry<float> minimumAudibleVolume;
    public ConfigEntry<bool> logSoundNames;

    //it's time to go the lazy route, add indicators with circles of varying colors and chars that go along with them

    private void Awake()
    {
        Instance ??= this;

        ManualLogSource = BepInEx.Logging.Logger.CreateLogSource(pluginGuid);
        ManualLogSource.LogInfo($"{pluginName} {pluginVersion} loaded!");

        //TODO setup config binding for more accesability functions
        // stole most of the boilerplate for this from justjelly.subtitles
        minimumAudibleVolume = Config.Bind<float>(
            section: "​Options",
            key: "MinimumAudibleVolume",
            defaultValue: 12f,
            description: "The minimum volume the mod determines is audible. Scale of 0-100. Any sound heard above this volume will be displayed as an indicator any sound below will not.");
        logSoundNames = Config.Bind<bool>(
            section: "Developers",
            key: "LogSoundNames",
            defaultValue: false,
            description: "Whether the mod should log the names of sounds");

        harmony = new Harmony(pluginGuid);
        harmony.PatchAll();
    }
}