using System;
using System.Text.RegularExpressions;
using HarmonyLib;
using RDLevelEditor;

namespace OldCharacterReplacer;

#pragma warning disable BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
public partial class OldCharacterReplacer
#pragma warning restore BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
{
    public class CharPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(scrChar), nameof(scrChar.Setup))]
        public static void SetupPrefix(ref Character character, ref string customCharacterName)
		{
			if (character == Character.Custom || customCharacterName != null)
				return;
			CharacterPlusCustom oldChar = OCRUtils.GetOldCharacter(character);
			if (oldChar.character != Character.Custom)
				return;
			character = oldChar.character;
			customCharacterName = oldChar.customCharacterName;
		}
    }
}

