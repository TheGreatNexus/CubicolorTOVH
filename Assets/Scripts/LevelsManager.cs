using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class LevelsManager : Manager<LevelsManager> {

	[Header("LevelsManager")]
	#region levels & current level management
	[SerializeField] GameObject[] m_LevelsPrefabs;

	int m_CurrentLevelIndex { get { return PlayerPrefs.GetInt("CURRENT_LEVEL_INDEX", 0); } set { PlayerPrefs.SetInt("CURRENT_LEVEL_INDEX", value); } }
	public int CurrentLevelIndex{get { return m_CurrentLevelIndex; }}

	//int m_LengthLevelsPrefabs { get { return m_LevelsPrefabs.Length; }}

	public int LevelsCount { get { return m_LevelsPrefabs.Length; } }

	int m_BestLevelIndex { get { return PlayerPrefs.GetInt("MAX_LEVEL_INDEX", 0); } set { PlayerPrefs.SetInt("MAX_LEVEL_INDEX", Mathf.Clamp(Mathf.Max(m_BestLevelIndex,value),0,m_LevelsPrefabs.Length-1)); } }
	public int BestLevelIndex { get { return m_BestLevelIndex; } }

	GameObject m_CurrentLevelGO;
	Level m_CurrentLevel;
	public Level CurrentLevel { get { return m_CurrentLevel; } }

	#endregion

	#region Manager implementation
	protected override IEnumerator InitCoroutine()
	{
		yield break;
	}
	#endregion

	#region Events' subscription
	public override void SubscribeEvents()
	{
		base.SubscribeEvents();
		EventManager.Instance.AddListener<GoToCurrentLevelEvent>(GoToCurrentLevel);
		EventManager.Instance.AddListener<GoToNextLevelEvent>(GoToNextLevel);
		EventManager.Instance.AddListener<SelectedLevelIndexHasChangedEvent>(SelectedLevelIndexHasChanged);
}

	public override void UnsubscribeEvents()
	{
		base.UnsubscribeEvents();
		EventManager.Instance.RemoveListener<GoToCurrentLevelEvent>(GoToCurrentLevel);
		EventManager.Instance.RemoveListener<GoToNextLevelEvent>(GoToNextLevel);
		EventManager.Instance.RemoveListener<SelectedLevelIndexHasChangedEvent>(SelectedLevelIndexHasChanged);
	}
	#endregion

	#region Level flow
	void Reset()
	{
		Destroy(m_CurrentLevelGO);
		m_CurrentLevelGO = null;
	}

	void InstantiateLevel(int levelIndex)
	{
		levelIndex = Mathf.Max(levelIndex, 0) % m_LevelsPrefabs.Length;
		m_CurrentLevelGO = Instantiate(m_LevelsPrefabs[levelIndex]);
		m_CurrentLevel = m_CurrentLevelGO.GetComponent<Level>();
	}

	private IEnumerator GoToCurrentLevelCoroutine()
	{
		Destroy(m_CurrentLevelGO);
		while (m_CurrentLevelGO) yield return null;

		InstantiateLevel(m_CurrentLevelIndex);
	}
	#endregion

	#region Callbacks to GameManager events
	protected override void GameMenu(GameMenuEvent e)
	{
		Reset();
	}
	protected override void GamePlay(GamePlayEvent e)
	{
		Reset();
	}

	public void GoToCurrentLevel(GoToCurrentLevelEvent e)
	{
		StartCoroutine(GoToCurrentLevelCoroutine());
	}

	public void GoToNextLevel(GoToNextLevelEvent e)
	{
		m_CurrentLevelIndex= (m_CurrentLevelIndex+1)%m_LevelsPrefabs.Length;
		m_BestLevelIndex = m_CurrentLevelIndex;
		StartCoroutine(GoToCurrentLevelCoroutine());
	}
	#endregion

	#region Callbacks to LevelSelectorUI events
	void SelectedLevelIndexHasChanged(SelectedLevelIndexHasChangedEvent e)
	{
		m_CurrentLevelIndex = e.eSelectedLevelIndex;
	}
	#endregion
}
