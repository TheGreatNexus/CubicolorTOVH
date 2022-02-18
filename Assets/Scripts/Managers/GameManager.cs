using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using System.Linq;

public enum GameState { gameMenu, gamePlay,gameInstantiateLevel,gamePause,gameOver,gameVictory}

public class GameManager : Manager<GameManager> {


	#region Time
	void SetTimeScale(float newTimeScale)
	{
		Time.timeScale = newTimeScale;
	}
	#endregion

	#region Game State
	private GameState m_GameState;
	public bool IsPlaying { get { return m_GameState == GameState.gamePlay; } }
	#endregion

	#region Events' subscription
	public override void SubscribeEvents()
	{
		base.SubscribeEvents();

		//MainMenuManager
		EventManager.Instance.AddListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
		EventManager.Instance.AddListener<PlayButtonClickedEvent>(PlayButtonClicked);
		EventManager.Instance.AddListener<NextLevelButtonClickedEvent>(NextLevelButtonClicked);
		EventManager.Instance.AddListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
		EventManager.Instance.AddListener<ReplayButtonClickedEvent>(ReplayButtonClicked);
		EventManager.Instance.AddListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
		EventManager.Instance.AddListener<QuitButtonClickedEvent>(QuitButtonClicked);

		//Level
		EventManager.Instance.AddListener<LevelHasBeenInstantiatedEvent>(LevelHasBeenInstantiated);
		EventManager.Instance.AddListener<AllTilesOfLevelHaveBeenDestroyedEvent>(AllTilesOfLevelHaveBeenDestroyed);
	}

	public override void UnsubscribeEvents()
	{
		base.UnsubscribeEvents();

		//MainMenuManager
		EventManager.Instance.RemoveListener<MainMenuButtonClickedEvent>(MainMenuButtonClicked);
		EventManager.Instance.RemoveListener<PlayButtonClickedEvent>(PlayButtonClicked);
		EventManager.Instance.RemoveListener<NextLevelButtonClickedEvent>(NextLevelButtonClicked);
		EventManager.Instance.RemoveListener<ResumeButtonClickedEvent>(ResumeButtonClicked);
		EventManager.Instance.RemoveListener<ReplayButtonClickedEvent>(ReplayButtonClicked);
		EventManager.Instance.RemoveListener<EscapeButtonClickedEvent>(EscapeButtonClicked);
		EventManager.Instance.RemoveListener<QuitButtonClickedEvent>(QuitButtonClicked);

		//Level
		EventManager.Instance.RemoveListener<LevelHasBeenInstantiatedEvent>(LevelHasBeenInstantiated);
		EventManager.Instance.RemoveListener<AllTilesOfLevelHaveBeenDestroyedEvent>(AllTilesOfLevelHaveBeenDestroyed);
	}
	#endregion

	#region Manager implementation
	protected override IEnumerator InitCoroutine()
	{
		Menu();
		yield break;
	}
	#endregion

	#region Game flow & Gameplay
	//Game initialization
	void InitNewGame()
	{
		m_GameState = GameState.gameInstantiateLevel; // le game state sera set à play après que le level est instantié
		EventManager.Instance.Raise(new GoToCurrentLevelEvent());
	}
	#endregion

	#region Callbacks to events issued by LevelManager
	private void LevelHasBeenInstantiated(LevelHasBeenInstantiatedEvent e)
	{
		SetTimeScale(1);
		m_GameState = GameState.gamePlay;
	}
	#endregion

	#region Callbacks to events issued by Level
	private void AllTilesOfLevelHaveBeenDestroyed(AllTilesOfLevelHaveBeenDestroyedEvent e)
	{
		SetTimeScale(0);
		m_GameState = GameState.gameInstantiateLevel;
		EventManager.Instance.Raise(new GoToNextLevelEvent());
	}
	#endregion

	#region Callbacks to Events issued by MenuManager
	private void MainMenuButtonClicked(MainMenuButtonClickedEvent e)
	{
		Menu();
	}

	private void PlayButtonClicked(PlayButtonClickedEvent e)
	{
		Play();
	}

	private void NextLevelButtonClicked(NextLevelButtonClickedEvent e)
	{
		EventManager.Instance.Raise(new GoToNextLevelEvent());
	}

	private void ResumeButtonClicked(ResumeButtonClickedEvent e)
	{
		Resume();
	}

	private void ReplayButtonClicked(ReplayButtonClickedEvent e)
	{
		Replay();
	}

	private void EscapeButtonClicked(EscapeButtonClickedEvent e)
	{
		if(IsPlaying)
			Pause();
	}

	private void QuitButtonClicked(QuitButtonClickedEvent e)
	{
		Application.Quit();
	}
	#endregion

	#region GameState methods
	private void Menu()
	{
		SetTimeScale(0);
		m_GameState = GameState.gameMenu;
		MusicLoopsManager.Instance.PlayMusic(Constants.MENU_MUSIC);
		EventManager.Instance.Raise(new GameMenuEvent());
	}

	private void Play()
	{
		m_GameState = GameState.gamePlay;
		MusicLoopsManager.Instance.PlayMusic(Constants.GAMEPLAY_MUSIC);
		EventManager.Instance.Raise(new GamePlayEvent());
		InitNewGame();
	}

	private void Replay()
	{
		m_GameState = GameState.gamePlay;
		EventManager.Instance.Raise(new GamePlayEvent());
		InitNewGame();
	}

	private void Pause()
	{
		SetTimeScale(0);
		m_GameState = GameState.gamePause;
		EventManager.Instance.Raise(new GamePauseEvent());
	}

	private void Resume()
	{
		SetTimeScale(1);
		m_GameState = GameState.gamePlay;
		EventManager.Instance.Raise(new GameResumeEvent());
	}

	private void Over()
	{
		SetTimeScale(0);
		m_GameState = GameState.gameOver;
		SfxManager.Instance.PlaySfx(Constants.GAMEOVER_SFX);
		EventManager.Instance.Raise(new GameOverEvent());
	}

	private void Victory()
	{
		SetTimeScale(0);
		m_GameState = GameState.gameVictory;
		SfxManager.Instance.PlaySfx(Constants.VICTORY_SFX);
		EventManager.Instance.Raise(new GameVictoryEvent());
	}
	#endregion
}
