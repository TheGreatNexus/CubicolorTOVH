using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SDD.Events;

public class HudManager : Manager<HudManager> {

	[Header("HudManager")]
	#region Labels & Values
	[Header("Texts")]
	[SerializeField] private Text m_LevelName;
	#endregion

	#region Manager implementation
	protected override IEnumerator InitCoroutine()
	{
		yield break;
	}
	#endregion

	#region Events subscription
	public override void SubscribeEvents()
	{
		base.SubscribeEvents();
	}
	public override void UnsubscribeEvents()
	{
		base.UnsubscribeEvents();
	}
	#endregion


	#region Callbacks to GameManager events
	protected override void GameStatisticsChanged(GameStatisticsChangedEvent e)
	{
		m_LevelName.text = e.eLevelName.ToString();
	}
	#endregion
}
