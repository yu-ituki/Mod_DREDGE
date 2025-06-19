using BepInEx;

using HarmonyLib;

using UnityEngine.Windows;
using InControl;
using System.Collections.Generic;

namespace Mod
{
	/// <summary>
	/// Modのエントリポイント.
	/// </summary>
	[BepInPlugin(ModInfo.c_ModFullName, ModInfo.c_ModName, ModInfo.c_ModVersion)]
	public class Plugin : BaseUnityPlugin
	{
		public static Plugin Instance { get; private set; }

		public ModConfig ModConfig { get => MyModManager.Instance.GetConfig() as ModConfig; }


		Mod_AbilityShortcut m_Shortcut;

		static readonly (UnityEngine.KeyCode, System.Action)[] c_ExKeys = new (UnityEngine.KeyCode, System.Action)[] {
			( UnityEngine.KeyCode.Alpha1, ()=>{   }),

		};
		

		/// <summary>
		/// Modのエントリポイント.
		/// </summary>
		private void Awake() {
			Instance = this;
			MyModManager.Instance.Initialize<ModConfig>(this, this.Logger, ModInfo.c_ModFullName, ModInfo.c_ModName, ModInfo.c_ModVersion);
			MyModManager.Instance.RegisterOnBootAction(OnBoot);

			m_Shortcut = new Mod_AbilityShortcut();
			m_Shortcut.Initialize();
		}

		/// <summary>
		/// Mod開放タイミング.
		/// </summary>
		void Unload() {
			MyModManager.Instance?.Terminate();
			MyModManager.DeleteInstance();
		}


		/// <summary>
		/// ゲーム起動時.
		/// </summary>
		void OnBoot() {
		//	var abilities = GameManager.Instance.PlayerAbilities;
		//	_AddInputKey("hoge.abi1", InControl.Key.Key1, () => { DebugUtil.LogWarning("key1!!!!"); });
		}



		public void Update() {
			m_Shortcut.Update();
		}
	}
}
