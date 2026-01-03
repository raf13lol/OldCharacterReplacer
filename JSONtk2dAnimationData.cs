using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OldCharacterReplacer;

public class JSONtk2dHeartAnimationDataHandler
{
    public static void ApplyAnimationDataToAnimator(tk2dSpriteAnimator animator, JSONtk2dAnimationData data)
    {
        foreach (KeyValuePair<string, JSONtk2dAnimationClip> kvpClip in data.clips)
        {
            tk2dSpriteAnimationClip tk2dClip = animator.Library.GetClipByName(kvpClip.Key);
			JSONtk2dAnimationClip clip = kvpClip.Value;
			tk2dClip.fps = clip.fps;
			
			for (int i = 0; i < clip.frames.Count; i++)
            {
                JSONtk2dAnimationFrame frame = clip.frames[i];

				if (i >= tk2dClip.frames.Length)
                {
					tk2dSpriteAnimationFrame lastFrame = tk2dClip.frames.Last();
                    tk2dSpriteAnimationFrame newFrame = new();

					newFrame.CopyFrom(lastFrame);
					lastFrame.ClearTrigger();

					tk2dClip.frames = [.. tk2dClip.frames, newFrame];
                }
				tk2dSpriteAnimationFrame tk2dFrame = tk2dClip.frames[i];
				tk2dSpriteDefinition tk2dDefinition = tk2dFrame.spriteCollection.spriteDefinitions[tk2dFrame.spriteId];

				float width = frame.width / (float)data.width;
				float height = frame.height / (float)data.height;
				Vector2 topLeftUV = new(frame.topLeftPosition[0] / (float)data.width, (data.height - frame.topLeftPosition[1] - frame.height) / (float)data.height);
				Vector2 topRightUV = new(topLeftUV.x + width, topLeftUV.y);
				Vector2 bottomLeftUV = new(topLeftUV.x, topLeftUV.y + height);
				Vector2 bottomRightUV = new(topLeftUV.x + width, topLeftUV.y + height);
				tk2dDefinition.uvs = [topLeftUV, topRightUV, bottomLeftUV, bottomRightUV];
		    }
        }
    }

	public static JSONtk2dAnimationData CreateAnimationDataFromAnimator(tk2dSpriteAnimator animator)
    {
		JSONtk2dAnimationData jsonData = new()
        {
			width = 1024,
			height = 2048,
			clips = []
        };

        foreach (tk2dSpriteAnimationClip clip in animator.Library.clips)
		{
			if (!clip.name.StartsWith("Heart"))
				continue;

			JSONtk2dAnimationClip jsonClip = new()
			{
				fps = (int)clip.fps,
				frames = []
			};

			foreach (tk2dSpriteAnimationFrame frame in clip.frames)
			{
				tk2dSpriteDefinition def = frame.spriteCollection.spriteDefinitions[frame.spriteId];
				JSONtk2dAnimationFrame jsonFrame = new()
				{
					spriteId = frame.spriteId,
					topLeftPosition = [Mathf.RoundToInt(def.uvs[0].x * jsonData.width), jsonData.height - Mathf.RoundToInt(def.uvs[0].y * jsonData.height) - (int)def.boundsData[1].y],
					width = (int)def.boundsData[1].x,
					height = (int)def.boundsData[1].y
				};
				jsonClip.frames.Add(jsonFrame);
			}    
			jsonData.clips[clip.name] = jsonClip;
		}

		return jsonData;
    }
}

public class JSONtk2dAnimationData
{
	public int width { get; set; }
	public int height { get; set; }
    public Dictionary<string, JSONtk2dAnimationClip> clips;
}

public class JSONtk2dAnimationClip
{
    public int fps { get; set; }
	public List<JSONtk2dAnimationFrame> frames { get; set; }
}

public class JSONtk2dAnimationFrame
{
    public List<int> topLeftPosition { get; set; }
	public int width { get; set; }
	public int height { get; set; }
	//! UNUSED
	public int spriteId { get; set; }
}