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
			new MyShortcut() { m_Key = config.Key_Boost.Value, m_ID = eAbility.Boost },       //< �u�[�X�g.
			new MyShortcut() { m_Key = config.Key_SpyGlass.Value, m_ID = eAbility.Spyglass },    //< �]����.
			new MyShortcut() { m_Key = config.Key_Light.Value, m_ID = eAbility.Lights },      //< ���C�g.
			new MyShortcut() { m_Key = config.Key_Trawl.Value, m_ID = eAbility.Trawl },       //< �g���[����.
			new MyShortcut() { m_Key = config.Key_Pot.Value, m_ID = eAbility.Pot },         //< �J�j����.
			new MyShortcut() { m_Key = config.Key_Banish.Value, m_ID = eAbility.Banish },      //< �o�j�b�V��.
			new MyShortcut() { m_Key = config.Key_Atrophy.Value, m_ID = eAbility.Atrophy },     //< �A�g���t�B.
			new MyShortcut() { m_Key = config.Key_Manifest.Value, m_ID = eAbility.Manifest },    //< �}�j�t�F�X�g.
			new MyShortcut() { m_Key = config.Key_Camera.Value, m_ID = eAbility.Camera },    //< �J�������[�h.
			new MyShortcut() { m_Key = config.Key_Foghorn.Value, m_ID = eAbility.Foghorn },    //< ���J.
		};


	}


	public void Update() {
		var gameMng = GameManager.Instance;
		var ctrl = gameMng?.Player?.Controller;
		if (ctrl == null)
			return;
		if (!ctrl.IsMovementAllowed)	//< �����˂�.
			return;

		// �V���g�J�`�F�b�N.
		var playerAbilities = gameMng.PlayerAbilities;
		var gameEvents = GameEvents.Instance;
		var config = Plugin.Instance.ModConfig;
		var saveData = GameManager.Instance.SaveData;
		for ( int i = 0; i < m_Shortcuts.Length; ++i) {
			// �V���g�J�������.
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

			// �I���ς݂��ۂ�.
			if (playerAbilities.CurrentlySelectedAbilityData == abilityData)
				break;

			// �I��.
			gameEvents.SelectPlayerAbility(abilityData);

			
			// �N�[���^�C���`�F�b�N.
			if (!config.IsNoneCooltime.Value) {
				if ( abilityData.cooldown > 0f) {
					var elapsedCastAbilityTime = playerAbilities.GetTimeSinceLastCast(abilityData);
					if (elapsedCastAbilityTime < abilityData.cooldown) {    //< �N�[���^�C����.
						break;
					}
				}
			}

			// �r���A�r���e�B�N�����ɕʂ̂��N�������狭���I�ɕ���.
			// (�o�ዾ�Ȃ�).
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

			// �u�[�X�g�͉E�N���b�N�Ŏg������...
			bool isToggleActive = true;
			if (ability as BoostAbility)
				isToggleActive = false;
			

			if ( isToggleActive ) {
				if (ability.IsActive) {
					ability.Deactivate();
				} else {
					ability.Activate();

					// �N�[���_�E���L�^.
					// NOTE: �Ȃ�Ō���������UI���Œ��ɃZ�[�u�f�[�^�������Ă񂾁c�H.
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


#if false //< �_���v�p.            
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
