using BepInEx.Configuration;

using System.Collections.Generic;

using UnityEngine;

namespace Mod
{
	/// <summary>
	/// Modコンフィグ用.
	/// </summary>
	public class ModConfig : ModConfigBase
	{
		public ConfigEntry<KeyCode> Key_Boost { get; set; }
		public ConfigEntry<KeyCode> Key_SpyGlass { get; set; }
		public ConfigEntry<KeyCode> Key_Light { get; set; }
		public ConfigEntry<KeyCode> Key_Trawl { get; set; }
		public ConfigEntry<KeyCode> Key_Pot { get; set; }
		public ConfigEntry<KeyCode> Key_Banish { get; set; }
		public ConfigEntry<KeyCode> Key_Atrophy { get; set; }

		public ConfigEntry<KeyCode> Key_Manifest { get; set; }
		public ConfigEntry<KeyCode> Key_Camera { get; set; }
		public ConfigEntry<KeyCode> Key_Foghorn { get; set; }
		

		public ConfigEntry<bool> IsNoneCooltime { get; set; }

		public override void Initialize( ConfigFile config )
		{
			Key_Boost = config.Bind("General", "Key_Boost", KeyCode.Alpha1, "shortcut key : boost.");
			Key_SpyGlass = config.Bind("General", "Key_SpyGlass", KeyCode.Alpha2, "shortcut key : spyglass.");
			Key_Light = config.Bind("General", "Key_Light", KeyCode.Alpha3, "shortcut key : light.");
			Key_Trawl = config.Bind("General", "Key_Trawl", KeyCode.Alpha4, "shortcut key : trawl.");
			Key_Pot = config.Bind("General", "Key_Pot", KeyCode.Alpha5, "shortcut key : crab pot.");
			Key_Banish = config.Bind("General", "Key_Banish", KeyCode.Alpha6, "shortcut key : banish.");
			Key_Atrophy = config.Bind("General", "Key_Atrophy", KeyCode.Alpha7, "shortcut key : atrophy.");
			Key_Manifest = config.Bind("General", "Key_Manifest", (KeyCode)(-1), "shortcut key : manifest.");
			Key_Camera = config.Bind("General", "Key_Camera", (KeyCode)(-1), "shortcut key : camera.");
			Key_Foghorn = config.Bind("General", "Key_Foghorn", (KeyCode)(-1), "shortcut key : foghorn.");
			IsNoneCooltime = config.Bind("General", "IsNoneCooltime", false, "is none ability cooltime.");
		}
	}
}

