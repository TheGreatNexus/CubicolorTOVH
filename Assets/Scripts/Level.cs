using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDD.Events;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour,IEventHandler{

	[SerializeField] Texture2D m_LevelTexture;
	[SerializeField] TileColorSO m_TileColors;
	[SerializeField] TileSetSO m_TileSet;

	[SerializeField] float m_MaxDistanceTileToPosition = .05f;

	static Vector3 TileOffsetPos = new Vector3(0, -.5f, 0);

	Vector3 m_CubeSpawnPosition;
	public Vector3 CubeSpawnPosition { get { return m_CubeSpawnPosition; } }

	List<Tile> m_Tiles = new List<Tile>();

	ExitTile m_ExitTile;
	List<ColorTile> m_ExitColorTiles = new List<ColorTile>();

	private void Awake()
	{
		SubscribeEvents();
	}
	private void OnDestroy()
	{
		UnsubscribeEvents();
	}

	Vector3 WorldTilePosition(int x,int y)
	{
		return  TileOffsetPos+new Vector3(x, 0, y) - new Vector3(m_LevelTexture.width / 2, 0, m_LevelTexture.height / 2);
	}

	private void Start()
	{
		for (int y = 0; y < m_LevelTexture.height; y++) 
		{
			for (int x = 0; x < m_LevelTexture.width; x++)
			{
				Color color = m_LevelTexture.GetPixel(x, y);
				COLOR logInColor;
				COLOR logOutColor;

				if (m_TileColors.IsCubeColor(color))
				{
					m_CubeSpawnPosition = WorldTilePosition(x,y)- TileOffsetPos;
					m_Tiles.Add(Instantiate(m_TileSet.NeutralTilePrefab, WorldTilePosition(x, y), Quaternion.identity, this.transform).GetComponent<Tile>());
					Debug.Log(y + " - " + x + " - " + "CUBE");
				}
				else if(m_TileColors.IsExitTileColor(color))
				{
					GameObject exitTileGO =  Instantiate(m_TileSet.ExitTilePrefab);
					m_ExitTile = exitTileGO.GetComponent<ExitTile>();
					exitTileGO.transform.position = WorldTilePosition(x, y) + Vector3.up* m_ExitTile.Height;
					exitTileGO.transform.SetParent(transform);
					m_Tiles.Add(m_ExitTile);
					Debug.Log(y + " - " + x + " - " + "EXIT TILE");
				}
				else if(m_TileColors.IsNeutralTileColor(color))
				{
					m_Tiles.Add(Instantiate(m_TileSet.NeutralTilePrefab, WorldTilePosition(x, y), Quaternion.identity, this.transform).GetComponent<Tile>());
					Debug.Log(y + " - " + x + " - " + "NEUTRAL TILE");
				}
				else if (m_TileColors.IsInTileColor(color,out logInColor))
				{
					InColorTile inColorTile =  Instantiate(m_TileSet.InColorTilePrefab, WorldTilePosition(x, y), Quaternion.identity, this.transform).GetComponent<InColorTile>();
					inColorTile.Color = logInColor;
					m_Tiles.Add(inColorTile);
					Debug.Log(y + " - " + x + " - " + logInColor +" IN TILE");
				}
				else if (m_TileColors.IsOutTileColor(color, out logOutColor))
				{
					OutColorTile outColorTile = Instantiate(m_TileSet.OutColorTilePrefab, WorldTilePosition(x, y), Quaternion.identity, this.transform).GetComponent<OutColorTile>();
					outColorTile.Color = logOutColor;
					m_Tiles.Add(outColorTile);
					Debug.Log(y + " - " + x + " - " + logOutColor + " OUT TILE");
				}
				else Debug.Log(y + " - " + x + " - " +"ERROR");
			}
		}

		OutColorTile[] outColorTiles = GetComponentsInChildren<OutColorTile>();
		foreach (var tile in outColorTiles)
		{
			GameObject colorTileGO = Instantiate(m_TileSet.ColorTilePrefab);
			colorTileGO.transform.SetParent(transform);
			ColorTile colorTile = colorTileGO.GetComponent<ColorTile>();
			colorTile.Color = tile.Color;
			colorTileGO.transform.position = m_ExitTile.transform.position;
			m_ExitTile.transform.position += Vector3.up * colorTile.Height;
			m_ExitColorTiles.Add(colorTile);
			m_Tiles.Add(colorTile);
		}

		EventManager.Instance.Raise(new LevelHasBeenInstantiatedEvent() { eLevel = this });
	}


	public void SubscribeEvents()
	{
		EventManager.Instance.AddListener<CubeHasLeftPositionEvent>(CubeHasLeftPosition);
		EventManager.Instance.AddListener<CubeHasReachedPositionEvent>(CubeHasReachedPosition);
		EventManager.Instance.AddListener<CubeHasCleanedAColorEvent>(CubeHasCleanedAColor);
	}

	public void UnsubscribeEvents()
	{
		EventManager.Instance.RemoveListener<CubeHasLeftPositionEvent>(CubeHasLeftPosition);
		EventManager.Instance.RemoveListener<CubeHasReachedPositionEvent>(CubeHasReachedPosition);
		EventManager.Instance.RemoveListener<CubeHasCleanedAColorEvent>(CubeHasCleanedAColor);
	}

	void CubeHasReachedPosition(CubeHasReachedPositionEvent e)
	{
		List<Tile> tiles = GetTilesAtPosition(e.ePos,0.01f);
		if (tiles != null && tiles.Count==1 )
		{
			Tile tile = tiles[0];

			if (tile is InColorTile)
				EventManager.Instance.Raise(new InColorTileHasBeenReachedEvent() { eColor = (tile as ColorTile).Color });
			else if (tile is OutColorTile)
			{
				EventManager.Instance.Raise(new OutColorTileHasBeenReachedEvent() { eColor = (tile as ColorTile).Color });
			}
			else if (tile is ExitTile)
			{
				EventManager.Instance.Raise(new AllTilesOfLevelHaveBeenDestroyedEvent());
			}
		}
	}

	void CubeHasLeftPosition(CubeHasLeftPositionEvent e)
	{
		List<Tile> tiles = GetTilesAtPosition(e.ePos);
		if(tiles!=null)
		{
			foreach (var tile in tiles)
			{
				tile.DestroyTile();
				m_Tiles.Remove(tile);
			}
		}
	}

	void CubeHasCleanedAColor(CubeHasCleanedAColorEvent e)
	{
		ColorTile colorTile = m_ExitColorTiles.Where(tile => tile.Color.Equals(e.eColor)).FirstOrDefault();
		if(colorTile!=null)
		{
			List<Tile> tilesToMove = m_ExitColorTiles.Where(tile => tile.transform.position.y > colorTile.transform.position.y).Cast<Tile>().ToList();
			foreach (var tile in tilesToMove)
				tile.TranslateTile(-Vector3.up * colorTile.Height);
			m_ExitTile.TranslateTile(-Vector3.up * colorTile.Height);
			m_ExitTile.RotateTile(Vector3.up, 90f);
			colorTile.DestroyTile(.25f,false);
			m_ExitColorTiles.Remove(colorTile);
			m_Tiles.Remove(colorTile);

			if (m_ExitColorTiles.Count == 0)
				Invoke("MoveDownExitTile", .75f);
		}
	}

	void MoveDownExitTile()
	{
		m_ExitTile.TranslateTile(-Vector3.up * m_ExitTile.Height);
	}


	List<Tile> GetTilesAtPosition(Vector3 pos,float maxVerticalDistance = float.PositiveInfinity)
	{
		Plane xzPlane = new Plane(Vector3.up, 0);

		return m_Tiles.Where(tile => Vector3.Distance(xzPlane.ClosestPointOnPlane(tile.transform.position), xzPlane.ClosestPointOnPlane(pos)) < m_MaxDistanceTileToPosition && Mathf.Abs(tile.transform.position.y+.5f)< maxVerticalDistance).Select(tile => tile).ToList();
	}

	public bool IsPathWalkable(Vector3 pos)
	{
		List<Tile> tiles = GetTilesAtPosition(pos);
		return tiles.Count == 1 && Mathf.Approximately(tiles[0].transform.position.y, -.5f);
	}
}
