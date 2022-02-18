﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using SDD.Events;

public class LevelSelectorUI :MonoBehaviour
{
	[SerializeField] ToggleGroup m_LevelSelectorToggleGroup;
	[SerializeField] string m_LevelToggleFormatString;

	void SetLevelParameters()
	{
		SetCurrentLevel(LevelsManager.Instance.CurrentLevelIndex);
		SetMaxLevel(LevelsManager.Instance.BestLevelIndex);
	}

	void SetCurrentLevel(int index)
	{
		m_LevelSelectorToggleGroup.SetAllTogglesOff();
		string toggleName = string.Format(m_LevelToggleFormatString, index+1);
		GetComponentsInChildren<Toggle>().Where(item => item.name.Equals(toggleName)).FirstOrDefault().isOn = true; 
	}

	void SetMaxLevel(int maxLevelIndex)
	{
		GetComponentsInChildren<Toggle>().ToList().ForEach(item => item.interactable = (int.Parse(item.name) - 1 <= maxLevelIndex));
	}

	IEnumerator Start()
	{
		while (!LevelsManager.Instance) yield return null;
		SetLevelParameters();
	}

	private void OnEnable()
	{
		if (!LevelsManager.Instance) return;
		SetLevelParameters();
	}

	public void SelectedLevelHasChanged(bool value)
	{
		if(value)
		{
			foreach(Toggle activeToggle in m_LevelSelectorToggleGroup.ActiveToggles())
				EventManager.Instance.Raise(new SelectedLevelIndexHasChangedEvent() {eSelectedLevelIndex = int.Parse(activeToggle.name)-1 });
		}
	}
}
