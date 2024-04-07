using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UIElements;
using static AudibleDistanceLib.AudibleDistanceLib;

namespace DirectionalIndicators.Patches;

//TODO look in to how voip gets implemented and see if it's possible to add an indicator for that

[HarmonyPatch(typeof(AudioSource))]
public class AudioSourcePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(AudioSource.PlayOneShotHelper), new[] { typeof(AudioSource), typeof(AudioClip), typeof(float) })]
    public static void PlayOneShotHelper_Prefix(AudioSource source, ref AudioClip clip, float volumeScale)
    {
        if (IsInWithinAudiableDistance(GameNetworkManager.Instance, source, volumeScale, Plugin.Instance.minimumAudibleVolume.Value))
        {
            AddIndicator(clip, source.transform);
        }
    }

    //[HarmonyPrefix]
    //[HarmonyPatch(nameof(AudioSource.Play), new[] { typeof(double) })]
    //public static void PlayDelayed_Prefix(AudioSource __instance)
    //{
    //    if (__instance.clip == null) return;
    //    if (IsInWithinAudiableDistance(GameNetworkManager.Instance, __instance, __instance.volume, Plugin.Instance.minimumAudibleVolume.Value))
    //    {
    //        AddIndicator(__instance.clip);
    //    }
    //}

    //[HarmonyPrefix]
    //[HarmonyPatch(nameof(AudioSource.Play), new System.Type[] { })]
    //public static void Play_Prefix(AudioSource __instance)
    //{
    //    if (__instance.clip == null) return;
    //    if (IsInWithinAudiableDistance(GameNetworkManager.Instance, __instance, __instance.volume, Plugin.Instance.minimumAudibleVolume.Value))
    //    {
    //        AddIndicator(__instance.clip);
    //    }
    //}

    private static void AddIndicator(AudioClip clip, Transform? sourceTransform)
    {
        if (clip?.name is null) return;

        string clipName = Path.GetFileNameWithoutExtension(clip.name);

        if (SoundInfo.Indicators.TryGetValue(clipName, out IndicatorType soundIndicator))
        {
            if (sourceTransform is Transform value)
            {
                Plugin.Instance.indicators.Add(new DirectionalIndicator(value, soundIndicator));
            }
        }
    }
}
