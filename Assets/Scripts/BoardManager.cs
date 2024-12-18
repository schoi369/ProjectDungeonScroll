using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
	public class CellData
	{
		public bool m_passable;
		public CellObject m_containedObject;
	}

	CellData[,] m_boardData;
	public Tilemap m_tilemap;
	public Grid m_grid;
	public List<Vector2Int> m_emptyCellsList;

	public int m_width;
	public int m_height;
	public Tile m_groundTile;
	public Tile m_blockingTile;

	public void Init()
	{
		m_emptyCellsList = new List<Vector2Int>();

		m_boardData = new CellData[m_width, m_height];

		for (int y = 0; y < m_height; y++)
		{
			for (int x = 0; x < m_width; x++)
			{
				Tile currentTile;
				m_boardData[x, y] = new CellData();

				if (x == 0 || y == 0 || x == m_width - 1 || y == m_height - 1)
				{
					currentTile = m_blockingTile;
					m_boardData[x, y].m_passable = false;
				}
				else
				{
					currentTile = m_groundTile;
					m_boardData[x, y].m_passable = true;
					m_emptyCellsList.Add(new Vector2Int(x, y));
				}

				SetCellTile(new Vector2Int(x, y), currentTile);
			}
		}

		m_emptyCellsList.Remove(new Vector2Int(1, 1)); // Player Cell Position
	}

	public void Clean()
	{
		if (m_boardData == null)
		{
			return;
		}

		for (int y = 0; y < m_height; y++)
		{
			for (int x = 0; x < m_width; x++)
			{
				var cellData = m_boardData[x, y];

				if (cellData.m_containedObject != null)
				{
					Destroy(cellData.m_containedObject.gameObject);
				}

				SetCellTile(new Vector2Int(x, y), null);
			}
		}
	}

	public Vector3 CellPosToWorldPos(Vector2Int a_cellPos)
	{
		return m_grid.GetCellCenterWorld((Vector3Int) a_cellPos);
	}

	public CellData GetCellData(Vector2Int a_cellPos)
	{
		if (a_cellPos.x < 0 || a_cellPos.x >= m_width || a_cellPos.y < 0 || a_cellPos.y >= m_height)
		{
			return null;
		}

		return m_boardData[a_cellPos.x, a_cellPos.y];
	}

	public void SetCellTile(Vector2Int a_cellPos, Tile tile)
	{
		m_tilemap.SetTile(new Vector3Int(a_cellPos.x, a_cellPos.y, 0), tile);
	}

	public Tile GetCellTile(Vector2Int a_cellPos)
	{
		return m_tilemap.GetTile<Tile>(new Vector3Int(a_cellPos.x, a_cellPos.y, 0));
	}
}
