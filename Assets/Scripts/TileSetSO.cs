using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "My Scriptable Objects/TileSetSO", fileName = "default tileset filename")]
public class TileSetSO : ScriptableObject
{
	[SerializeField] GameObject m_NeutralTilePrefab;
	public GameObject NeutralTilePrefab { get { return m_NeutralTilePrefab; } }
	[SerializeField] GameObject m_InColorTilePrefab;
	public GameObject InColorTilePrefab { get { return m_InColorTilePrefab; } }
	[SerializeField] GameObject m_OutColorTilePrefab;
	public GameObject OutColorTilePrefab { get { return m_OutColorTilePrefab; } }
	[SerializeField] GameObject m_ColorTilePrefab;
	public GameObject ColorTilePrefab { get { return m_ColorTilePrefab; } }
	[SerializeField] GameObject m_ExitTilePrefab;
	public GameObject ExitTilePrefab { get { return m_ExitTilePrefab; } }
}
