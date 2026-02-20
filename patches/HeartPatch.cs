using System.IO;
using HarmonyLib;
using Newtonsoft.Json;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace OldCharacterReplacer;

#pragma warning disable BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
public partial class OldCharacterReplacer
#pragma warning restore BepInEx002 // Classes with BepInPlugin attribute must inherit from BaseUnityPlugin
{
	public class HeartPatch
	{
		public static Texture NewHeart = null;
		public static Texture NewHeartOutline;
		public static Texture NewHeartGlow;
		public static bool NeedsRevertingToNewHeart = false;

		public static JSONtk2dAnimationData OldHeartAnimationData = JsonConvert.DeserializeObject<JSONtk2dAnimationData>(File.ReadAllText(Path.Combine(path, "oldHeart", "oldHeart.json")));
		public static JSONtk2dAnimationData NewHeartAnimationData;

		[HarmonyPrefix]
		[HarmonyPatch(typeof(scrHeart), nameof(scrHeart.Setup))]
		public static void HeartPrefix(scrHeart __instance)
		{
			if (!OCRUtils.IsCustomLevel() || OCRUtils.GetVersion() > 64)
			{
				if (NeedsRevertingToNewHeart)
				{
					__instance.sprite.CurrentSprite.material.mainTexture = NewHeart;
					__instance.sprite.CurrentSprite.material.SetTexture(RDShaderProperties.OutlineTexProperty, NewHeartOutline);
					__instance.sprite.CurrentSprite.material.SetTexture(RDShaderProperties.GlowTexProperty, NewHeartGlow);
					JSONtk2dHeartAnimationDataHandler.ApplyAnimationDataToAnimator(__instance.animator, NewHeartAnimationData);
					NeedsRevertingToNewHeart = false;
				}
				return;
			}
			
			Texture2D img = LoadTextureFileToCache(Path.Combine("oldHeart", "oldHeart.png"), "oldHeartNormal", false);
			Texture2D outline = LoadTextureFileToCache(Path.Combine("oldHeart", "oldHeart_outline.png"), "oldHeartOutline", true);
			Texture2D glow = LoadTextureFileToCache(Path.Combine("oldHeart", "oldHeart_glow.png"), "oldHeartGlow", true);

			img.filterMode = outline.filterMode = glow.filterMode = FilterMode.Point;
			img.wrapMode = outline.wrapMode = glow.wrapMode = TextureWrapMode.Clamp;
			if (NewHeart == null)
			{
				NewHeart = __instance.sprite.CurrentSprite.material.mainTexture;
				NewHeartOutline = __instance.sprite.CurrentSprite.material.GetTexture(RDShaderProperties.OutlineTexProperty);
				NewHeartGlow = __instance.sprite.CurrentSprite.material.GetTexture(RDShaderProperties.GlowTexProperty);
				NewHeartAnimationData = JSONtk2dHeartAnimationDataHandler.CreateAnimationDataFromAnimator(__instance.animator);
			}
			
			JSONtk2dHeartAnimationDataHandler.ApplyAnimationDataToAnimator(__instance.animator, OldHeartAnimationData);
			__instance.sprite.CurrentSprite.material.mainTexture = img;
			__instance.sprite.CurrentSprite.material.SetTexture(RDShaderProperties.OutlineTexProperty, outline);
			__instance.sprite.CurrentSprite.material.SetTexture(RDShaderProperties.GlowTexProperty, glow);
			NeedsRevertingToNewHeart = true;

		}
	
		[HarmonyPostfix]
		[HarmonyPatch(typeof(scrHeart), "Update")]
		public static void Postfix(scrHeart __instance)
        {
			__instance.shaderDataToUse.SetPropertiesInRenderer(__instance.shaderRenderer);
			// __instance.sprite.CurrentSprite.material.SetInt(RDShaderProperties.OutlineProperty, 1);
			// logger.LogMessage(__instance.sprite.CurrentSprite.material.GetInt(RDShaderProperties.OutlineProperty));
			// logger.LogMessage(__instance.sprite.CurrentSprite.material.GetColor(RDShaderProperties.GlowColorProperty));
        }
	}
}

