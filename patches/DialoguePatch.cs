using HarmonyLib;
using System;

namespace OldCharacterReplacer;

#pragma warning disable BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
public partial class OldCharacterReplacer
#pragma warning restore BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
{
	[HarmonyPatch(typeof(RDInk), nameof(RDInk.ParsePortrait))]
	public class DialoguePatch
	{
		public static void Postfix(string fullName, ref bool isInternal, ref string charName, ref string expression)
		{
			if (!OCRUtils.IsCustomLevel() || !isInternal)
				return;

			int exprIndex = fullName.IndexOf('_');
			if (charName == Character.Paige.ToString() && exprIndex == -1 && OCRUtils.GetVersion() < 39)
				expression = "conversing";
			Character character = Enum.Parse<Character>(charName, true);
			CharacterPlusCustom oldChar = OCRUtils.GetOldCharacter(character);
			if (oldChar.character != Character.Custom)
				return;
			charName = oldChar.customCharacterName;
			isInternal = false;
		}
	}
}

