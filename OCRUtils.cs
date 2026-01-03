using System.Collections.Generic;
using System.IO;
using RDLevelEditor;
using UnityEngine;

namespace OldCharacterReplacer;

public class OCRUtils
{
    // TODO: Get a proper date on:
    // TODO: - Miner -> 43 -> olderMiner
    public static List<object[]> chars = [
        // these are just for dialogue portraits
        [Character.HoodieBoy, 30, Character.Custom, "olderCole"],
        [Character.Farmer, 30, Character.Custom, "oldFarmer"],
        
        // this is when early access happened so massive spike here in changes
        [Character.Samurai, 43, Character.Custom, "oldSamurai"],
        [Character.SamuraiBoss, 43, Character.Custom, "oldInsomniac"],
        [Character.Boy, 43, Character.Custom, "oldLogan"],
        [Character.Girl, 43, Character.Custom, "oldHailey"],
        [Character.HoodieBoy, 43, Character.Custom, "oldCole"],
        [Character.HoodieBoyBlue, 43, Character.Custom, "oldColeBlue"],
        [Character.Miner, 43, Character.Custom, "olderMiner"],
        [Character.Ian, 43, Character.Custom, "oldIan"],
        
        [Character.Bodybuilder, 55, Character.Custom, "oldBodybuilder"],
        
        [Character.Miner, 56, Character.Custom, "oldMiner"],
        [Character.Paige, 56, Character.Custom, "olderPaige"],

        [Character.Paige, 65, Character.Custom, "oldPaige"],
    ];

	public static int GetVersion()
		=> OldCharacterReplacer.overrideVersion.Value != 0 ? OldCharacterReplacer.overrideVersion.Value : RDLevelData.current.settings.version;

    public static CharacterPlusCustom GetOldCharacter(Character character)
    {
        if (!IsCustomLevel())
            return new(character, null);

        Character oldChar = character;
        string customPath = null;
        int version = GetVersion();

        foreach (var list in chars)
        {
            if (character != (Character)list[0])
                continue;

            if (version >= (int)list[1])
                continue;

            oldChar = (Character)list[2];
            customPath = (string)list[3];
            break;
        }

        if (customPath != null)
            CreateCustomCharacter(customPath);
        return new(oldChar, OldCharacterReplacer.dictionaryPrefix + customPath);
    }

    // LevelEvent_MakeRow.UpdateCustomCharacter
    public static void CreateCustomCharacter(string name)
    {
        if (!IsCustomLevel())
            return;

        var ccd = scnGame.instance.currentLevel.customCharacterData;
        var key = OldCharacterReplacer.dictionaryPrefix + name;
        if (ccd.ContainsKey(key))
            return;

        Texture2D img = OldCharacterReplacer.LoadTextureFileToCache(Path.Combine(name, $"{name}.png"), name + "normal", false);
        Texture2D outline = OldCharacterReplacer.LoadTextureFileToCache(Path.Combine(name, $"{name}_outline.png"), name + "outline", true);
        Texture2D glow = OldCharacterReplacer.LoadTextureFileToCache(Path.Combine(name, $"{name}_glow.png"), name + "glow", true);
        Texture2D freeze = OldCharacterReplacer.LoadTextureFileToCache(Path.Combine(name, $"{name}_freeze.png"), name + "freeze", true);

        string jsonTxt = File.ReadAllText(Path.Combine(OldCharacterReplacer.path, name, $"{name}.json"));

        img.filterMode = outline.filterMode = glow.filterMode = freeze.filterMode = FilterMode.Point;
        img.wrapMode = outline.wrapMode = glow.wrapMode = freeze.wrapMode = TextureWrapMode.Clamp;

        LevelBase.LoadCustomCharacter(ccd, key, jsonTxt, img, outline, glow, freeze);
    }


    public static bool IsCustomLevel()
    	=> OldCharacterReplacer.overrideVersion.Value != 0 || scnGame.levelToLoadSource == LevelSource.ExternalPath;
}

public struct CharacterPlusCustom(Character chr, string ccp)
{
    public Character character = chr;
    public string customCharacterName = ccp;
}