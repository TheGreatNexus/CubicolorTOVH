using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "My Scriptable Objects/TileColorSO", fileName = "default tilecolor filename")]
public class TileColorSO : ScriptableObject
{
	[SerializeField] Color m_CubeTileColor;
	public Color CubeTileColor { get { return m_CubeTileColor; } }
	[SerializeField] Color m_NeutralTileColor;
	public Color NeutralTileColor { get { return m_NeutralTileColor; } }
	[SerializeField] Color m_ExitTileColor;
	public Color ExitTileColor { get { return m_ExitTileColor; } }
	[SerializeField] Color[] m_InTileColors;
	[SerializeField] Color[] m_OutTileColors;

	public bool IsCubeColor(Color color)
	{
		return color == m_CubeTileColor;
	}

	public bool IsNeutralTileColor(Color color)
	{
		return color == m_NeutralTileColor;
	}

	public bool IsExitTileColor(Color color)
	{
		return color == m_ExitTileColor;
	}

	public bool IsInTileColor(Color color,out COLOR logInColor)
	{
		logInColor = COLOR.black;
		for (int i = 0; i < m_InTileColors.Length; i++)
		{
			if(color == m_InTileColors[i])
			{
				logInColor = (COLOR)(i + 1);
				return true;
			}
		}
		return false;
	}

	public bool IsOutTileColor(Color color, out COLOR logOutColor)
	{
		logOutColor = COLOR.black;
		for (int i = 0; i < m_OutTileColors.Length; i++)
		{
			if (color == m_OutTileColors[i])
			{
				logOutColor = (COLOR)(i + 1);
				return true;
			}
		}
		return false;
	}
}
