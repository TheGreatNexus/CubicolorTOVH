using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

public class MenuManager : Manager<MenuManager> {

	[Header("MenuManager")]

	#region Panels
	[Header("Panels")]
	[SerializeField] GameObject m_PanelMainMenu;
	[SerializeField] GameObject m_PanelInGameMenu;
	[SerializeField] GameObject m_PanelNextLevel;
	[SerializeField] GameObject m_PanelVictory;
	[SerializeField] GameObject m_PanelGameOver;
	[SerializeField] GameObject m_PanelHUD;

	List<GameObject> m_AllPanels;
	#endregion

	#region Events' subscription
	public override void SubscribeEvents()
	{
		base.SubscribeEvents();

		//GameManager
		EventManager.Instance.AddListener<AskToGoToNextLevelEvent>(AskToGoToNextLevel);
		EventManager.Instance.AddListener<GoToNextLevelEvent>(GoToNextLevel);
	}

	public override void UnsubscribeEvents()
	{
		base.UnsubscribeEvents();

		//GameManager
		EventManager.Instance.RemoveListener<AskToGoToNextLevelEvent>(AskToGoToNextLevel);
		EventManager.Instance.RemoveListener<GoToNextLevelEvent>(GoToNextLevel);
	}
	#endregion

	#region Manager implementation
	protected override IEnumerator InitCoroutine()
	{
		yield break;
	}
	#endregion

	#region Monobehaviour lifecycle
	protected override void Awake()
	{
		base.Awake();
		RegisterPanels();
	}

	private void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			EscapeButtonHasBeenClicked();
		}
	}
	#endregion

	#region Panel Methods
	void RegisterPanels()
	{
		m_AllPanels = new List<GameObject>();
		m_AllPanels.Add(m_PanelMainMenu);
		m_AllPanels.Add(m_PanelInGameMenu);
		m_AllPanels.Add(m_PanelNextLevel);
		m_AllPanels.Add(m_PanelVictory);
		m_AllPanels.Add(m_PanelGameOver);
		m_AllPanels.Add(m_PanelHUD);
	}

	void OpenPanel(GameObject panel)
	{
		foreach (var item in m_AllPanels)
			if(item) item.SetActive(item == panel);
	}
	#endregion

	#region UI OnClick Events
	public void EscapeButtonHasBeenClicked()
	{
		EventManager.Instance.Raise(new EscapeButtonClickedEvent());
	}

	public void PlayButtonHasBeenClicked()
	{
		EventManager.Instance.Raise(new PlayButtonClickedEvent());
	}

	public void ResumeButtonHasBeenClicked()
	{
		EventManager.Instance.Raise(new ResumeButtonClickedEvent());
	}

	public void ReplayButtonHasBeenClicked()
	{
		EventManager.Instance.Raise(new ReplayButtonClickedEvent());
	}

	public void MainMenuButtonHasBeenClicked()
	{
		EventManager.Instance.Raise(new MainMenuButtonClickedEvent());
	}

	public void NextLevelButtonHasBeenClicked()
	{
		EventManager.Instance.Raise(new NextLevelButtonClickedEvent());
	}

	public void QuitButtonHasBeenClicked()
	{
		EventManager.Instance.Raise(new QuitButtonClickedEvent());
	}
	#endregion

	#region Callbacks to GameManager events
	private void AskToGoToNextLevel(AskToGoToNextLevelEvent e)
	{
		OpenPanel(m_PanelNextLevel);
	}

	private void GoToNextLevel (GoToNextLevelEvent e)
	{
		OpenPanel(null);
	}

	protected override void GameMenu(GameMenuEvent e)
	{
		OpenPanel(m_PanelMainMenu);
	}

	protected override void GamePlay(GamePlayEvent e)
	{
		OpenPanel(m_PanelHUD);
	}

	protected override void GamePause(GamePauseEvent e)
	{
		OpenPanel(m_PanelInGameMenu);
	}

	protected override void GameResume(GameResumeEvent e)
	{
		OpenPanel(null);
	}

	protected override void GameOver(GameOverEvent e)
	{
		OpenPanel(m_PanelGameOver);
	}

	protected override void GameVictory(GameVictoryEvent e)
	{
		OpenPanel(m_PanelVictory);
	}
	#endregion
}
