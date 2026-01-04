using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace OldCharacterReplacer;

#pragma warning disable BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
public partial class OldCharacterReplacer
#pragma warning restore BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
{
	[HarmonyPatch(typeof(scrChar), nameof(scrChar.Setup))]
    public class CharPatch
    {
        public static void Prefix(ref Character character, ref string customCharacterName, ref string expression)
		{
			if (scnGame.instance == null || character == Character.Custom || customCharacterName != null)
				return;
			CharacterPlusCustom oldChar = OCRUtils.GetOldCharacter(character);
			if (oldChar.character != Character.Custom)
				return;
			character = oldChar.character;
			customCharacterName = oldChar.customCharacterName;
		}
	}
}

