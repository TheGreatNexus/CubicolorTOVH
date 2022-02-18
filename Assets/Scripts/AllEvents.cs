using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;

#region GameManager Events
public class GameMenuEvent : SDD.Events.Event
{
}
public class GamePlayEvent : SDD.Events.Event
{
}
public class GamePauseEvent : SDD.Events.Event
{
}
public class GameResumeEvent : SDD.Events.Event
{
}
public class GameOverEvent : SDD.Events.Event
{
}
public class GameVictoryEvent : SDD.Events.Event
{
}
public class GameStatisticsChangedEvent : SDD.Events.Event
{
	public int eBestScore { get; set; }
	public int eScore { get; set; }
	public int eNLives { get; set; }
	public int eNEnemiesLeftBeforeVictory { get; set; }
}
#endregion

#region MenuManager Events
public class EscapeButtonClickedEvent : SDD.Events.Event
{
}
public class PlayButtonClickedEvent : SDD.Events.Event
{
}
public class ResumeButtonClickedEvent : SDD.Events.Event
{
}
public class ReplayButtonClickedEvent : SDD.Events.Event
{
}
public class MainMenuButtonClickedEvent : SDD.Events.Event
{
}
public class NextLevelButtonClickedEvent : SDD.Events.Event
{
}

public class QuitButtonClickedEvent : SDD.Events.Event
{
}
#endregion

#region Game Manager Additional Event
public class AskToGoToNextLevelEvent : SDD.Events.Event
{
}
public class GoToNextLevelEvent : SDD.Events.Event
{
}
public class GoToCurrentLevelEvent : SDD.Events.Event
{
}
#endregion

#region Cube Events
public class CubeHasLeftPositionEvent:SDD.Events.Event
{
	public Vector3 ePos;
}
public class CubeHasReachedPositionEvent : SDD.Events.Event
{
	public Vector3 ePos;
}
public class CubeHasCleanedAColorEvent:SDD.Events.Event
{
	public COLOR eColor;
}
#endregion

#region Level Events
public class AllTilesOfLevelHaveBeenDestroyedEvent : SDD.Events.Event
{
}

public class InColorTileHasBeenReachedEvent:SDD.Events.Event
{
	public COLOR eColor;
}
public class OutColorTileHasBeenReachedEvent : SDD.Events.Event
{
	public COLOR eColor;
}
#endregion

#region LevelsManager Events
public class LevelHasBeenInstantiatedEvent : SDD.Events.Event
{
	public Level eLevel;
}
#endregion

#region Level Selector UI
public class SelectedLevelIndexHasChangedEvent:SDD.Events.Event
{
	public int eSelectedLevelIndex;
}
#endregion