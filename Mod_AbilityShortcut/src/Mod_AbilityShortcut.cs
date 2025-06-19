using HarmonyLib;
using Mod;

using System;
using System.Security.AccessControl;
using System.Xml.Linq;


[HarmonyPatch]
class Mod_AbilityShortcut
{
	public enum eAbility
	{
		Boost,
		Spyglass,
		Foghorn,
		Manifest,
		Lights,
		Trawl,
		Pot,
		Banish,
		Bait,
		Atrophy,
		Camera,

		MAX
	}

	class AbilityInfo {
		public eAbility m_ID;
		public string m_Name;
		public System.Type m_ClassType;
	}


	class MyShortcut {
		public eAbility m_ID;
		public UnityEngine.KeyCode m_Key;
	}


	static readonly AbilityInfo[] c_AbilityInfos = new AbilityInfo[] {
		new AbilityInfo(){ m_ID = eAbility.Boost, m_ClassType = typeof(BoostAbility), m_Name = "haste"},
		new AbilityInfo(){ m_ID = eAbility.Spyglass, m_ClassType = typeof(SpyglassAbility), m_Name = "spyglass"},
		new AbilityInfo(){ m_ID = eAbility.Foghorn, m_ClassType = typeof(FoghornAbility), m_Name = "foghorn"},
		new AbilityInfo(){ m_ID = eAbility.Manifest, m_ClassType = typeof(TeleportAbility), m_Name = "spyglass"},
		new AbilityInfo(){ m_ID = eAbility.Lights, m_ClassType = typeof(LightAbility), m_Name = "lights"},
		new AbilityInfo(){ m_ID = eAbility.Trawl, m_ClassType = typeof(TrawlNetAbility), m_Name = "trawl"},
		new AbilityInfo(){ m_ID = eAbility.Pot, m_ClassType = typeof(DeployPotAbility), m_Name = "pot"},
		new AbilityInfo(){ m_ID = eAbility.Banish, m_ClassType = typeof(BanishAbility), m_Name = "banish"},
		new AbilityInfo(){ m_ID = eAbility.Bait, m_ClassType = typeof(BaitAbility), m_Name = "bait"},
		new AbilityInfo(){ m_ID = eAbility.Atrophy, m_ClassType = typeof(AtrophyAbility), m_Name = "atrophy"},
		new AbilityInfo(){ m_ID = eAbility.Camera, m_ClassType = typeof(CameraAbility), m_Name = "camera"},
	};

	static readonly eAbility[] c_ExclusiveAbilities = new eAbility[] {
		eAbility.Spyglass,
		eAbility.Camera,
	};

	MyShortcut[] m_Shortcuts;



	public void Initialize() {
		var config = Plugin.Instance.ModConfig;

		m_Shortcuts = new MyShortcut[] {
			new MyShortcut() { m_Key = config.Key_Boost.Value, m_ID = eAbility.Boost },       //< ブースト.
			new MyShortcut() { m_Key = config.Key_SpyGlass.Value, m_ID = eAbility.Spyglass },    //< 望遠鏡.
			new MyShortcut() { m_Key = config.Key_Light.Value, m_ID = eAbility.Lights },      //< ライト.
			new MyShortcut() { m_Key = config.Key_Trawl.Value, m_ID = eAbility.Trawl },       //< トロール網.
			new MyShortcut() { m_Key = config.Key_Pot.Value, m_ID = eAbility.Pot },         //< カニかご.
			new MyShortcut() { m_Key = config.Key_Banish.Value, m_ID = eAbility.Banish },      //< バニッシュ.
			new MyShortcut() { m_Key = config.Key_Atrophy.Value, m_ID = eAbility.Atrophy },     //< アトロフィ.
			new MyShortcut() { m_Key = config.Key_Manifest.Value, m_ID = eAbility.Manifest },    //< マニフェスト.
			new MyShortcut() { m_Key = config.Key_Camera.Value, m_ID = eAbility.Camera },    //< カメラモード.
			new MyShortcut() { m_Key = config.Key_Foghorn.Value, m_ID = eAbility.Foghorn },    //< 霧笛.
		};


	}


	public void Update() {
		var gameMng = GameManager.Instance;
		var ctrl = gameMng?.Player?.Controller;
		if (ctrl == null)
			return;
		if (!ctrl.IsMovementAllowed)	//< 動けねえ.
			return;

		// ショトカチェック.
		var playerAbilities = gameMng.PlayerAbilities;
		var gameEvents = GameEvents.Instance;
		var config = Plugin.Instance.ModConfig;
		var saveData = GameManager.Instance.SaveData;
		for ( int i = 0; i < m_Shortcuts.Length; ++i) {
			// ショトカ無効状態.
			if (m_Shortcuts[i].m_Key == (UnityEngine.KeyCode)(-1))
				continue;

			if (!UnityEngine.Input.GetKeyDown(m_Shortcuts[i].m_Key))
				continue;

			var abilityInfo = _FindAbilityInfo(m_Shortcuts[i].m_ID);
			var abilityData = playerAbilities.GetAbilityDataByName(abilityInfo.m_Name);
			if (abilityData == null)
				continue;
			var ability = playerAbilities.GetAbilityForData(abilityData);
			if (ability == null)
				continue;

			// 選択済みか否か.
			if (playerAbilities.CurrentlySelectedAbilityData == abilityData)
				break;

			// 選択.
			gameEvents.SelectPlayerAbility(abilityData);

			
			// クールタイムチェック.
			if (!config.IsNoneCooltime.Value) {
				if ( abilityData.cooldown > 0f) {
					var elapsedCastAbilityTime = playerAbilities.GetTimeSinceLastCast(abilityData);
					if (elapsedCastAbilityTime < abilityData.cooldown) {    //< クールタイム中.
						break;
					}
				}
			}

			// 排他アビリティ起動中に別のが起動したら強制的に閉じる.
			// (双眼鏡など).
			var currentAbilityType = ability.GetType();
			for ( int j = 0; j < c_ExclusiveAbilities.Length; ++j) {
				var exclusiveAbilityInfo = _FindAbilityInfo(m_Shortcuts[j].m_ID);
				if (currentAbilityType == exclusiveAbilityInfo.m_ClassType)
					continue;
				var exclusiveData = playerAbilities.GetAbilityDataByName(exclusiveAbilityInfo.m_Name);
				var exclusiveAbility = playerAbilities.GetAbilityForData(exclusiveData);
				if (exclusiveAbility.IsActive) {
					exclusiveAbility.Deactivate();
				}
			}

			// ブーストは右クリックで使いたい...
			bool isToggleActive = true;
			if (ability as BoostAbility)
				isToggleActive = false;
			

			if ( isToggleActive ) {
				if (ability.IsActive) {
					ability.Deactivate();
				} else {
					ability.Activate();

					// クールダウン記録.
					// NOTE: なんで元処理だとUI側で直にセーブデータいじってんだ…？.
					string key = abilityData.name.ToLowerInvariant();
					if (saveData.abilityHistory.ContainsKey(key)) {
						saveData.abilityHistory[key] = gameMng.Time.TimeAndDay;
					} else {
						saveData.abilityHistory.Add(key, gameMng.Time.TimeAndDay);
					}
				}
			}
			break;
		}


	}


#if false //< ダンプ用.            
	[HarmonyPatch(typeof(Ability), "OnPlayerAbilitySelected")]
	[HarmonyPostfix]
	static void Test(AbilityData selectedAbilityData, Ability __instance) {
		DebugUtil.LogWarning(__instance.AbilityData.name);
	}
#endif


	static AbilityInfo _FindAbilityInfo(eAbility id) {
		for (int i = 0; i < c_AbilityInfos.Length; ++i)
			if (c_AbilityInfos[i].m_ID == id)
				return c_AbilityInfos[i];
		return null;
	}
}
