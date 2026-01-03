using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
#if !BPE5
using BepInEx.Unity.Mono;
#endif
using HarmonyLib;
using System;
using System.IO;
using UnityEngine;
using RDLevelEditor;
using System.Reflection;

namespace OldCharacterReplacer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Rhythm Doctor.exe")]
// i don't know what
#pragma warning disable BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
public partial class OldCharacterReplacer : BaseUnityPlugin
#pragma warning restore BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
{
    public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BepInEx", "plugins", "ocrData");
    // illegal characters
    public static string dictionaryPrefix = @"|?#<OCR>#?|\/";
    public static MethodInfo loadTexture2D;

    internal static ManualLogSource logger;

    private ConfigEntry<bool> charReplacement;
    public static ConfigEntry<int> overrideVersion;

    private void Awake()
    {
        logger = Logger;
        loadTexture2D = typeof(RDEditorUtils).GetMethod("LoadTextureFileToCache", BindingFlags.Static | BindingFlags.NonPublic);

        charReplacement = Config.Bind("General", "ReplaceCharacters", true, "If characters should be replaced with older versions according to the level version.");
        overrideVersion = Config.Bind("General", "OverrideVersion", 0, "If the value is not zero, every level (including story levels and the editor) will be treated as being from that version.");
        logger.LogMessage($"Old Character Replacer plugin loaded! Will {(charReplacement.Value ? "" : "not ")}replace characters with older versions when appropriate!");

        Harmony instance = new("patcher");
        if (charReplacement.Value)
        {
            instance.PatchAll(typeof(CharPatch));
            instance.PatchAll(typeof(DialoguePatch));
            instance.PatchAll(typeof(HeartPatch));
        }
    }

    public static Texture2D LoadTextureFileToCache(string p, string key, bool isAlpha8)
    {
        if (scrVfxControl.textureCache.TryGetValue(key, out CachedTexture tex))
            return tex.texture;
        return (Texture2D)loadTexture2D.Invoke(null, [scrVfxControl.textureCache, Path.Combine(path, p), dictionaryPrefix + key, isAlpha8]);
    }
}