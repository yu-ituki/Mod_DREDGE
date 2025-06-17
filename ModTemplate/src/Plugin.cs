using BepInEx;

using HarmonyLib;

using UnityEngine.Windows;

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


		/// <summary>
		/// Modのエントリポイント.
		/// </summary>
		private void Awake() {
			Instance = this;
			MyModManager.Instance.Initialize<ModConfig>(this, this.Logger, ModInfo.c_ModFullName, ModInfo.c_ModName, ModInfo.c_ModVersion);
			MyModManager.Instance.RegisterOnBootAction(OnBoot);
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
			DebugUtil.LogWarning("!!!! Boot");
		}

		


#if false
		public void Update() {
		}
#endif
	}
}
