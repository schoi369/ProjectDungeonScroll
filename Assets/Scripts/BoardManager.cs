using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
	public class CellData
	{
		public GroundTile m_groundTile;
		public CellObject m_containedObject;

		public bool Passable
		{
			get
			{
				bool result = true;
				result &= m_groundTile.m_passable;
				return result;
			}
		}

		public void CleanUp()
		{
			if (m_groundTile)
			{
				Destroy(m_groundTile.gameObject);
			}

			if (m_containedObject)
			{
				Destroy(m_containedObject.gameObject);
			}

			m_groundTile = null;
			m_containedObject = null;
		}
	}

	public Transform m_boardHolder;

	CellData[,] m_boardData;
	public List<Vector2Int> m_emptyCellPositionList;

	public int m_width;
	public int m_height;
	public float m_cellSize = 1f;

	public GroundTile m_prefabGroundTile;

	public void Init()
	{
		m_emptyCellPositionList = new List<Vector2Int>();

		m_boardData = new CellData[m_width, m_height];

		for (int y = 0; y < m_height; y++)
		{
			for (int x = 0; x < m_width; x++)
			{
				var cellPos = new Vector2Int(x, y);

				SetCellTile(cellPos, m_prefabGroundTile);
				m_emptyCellPositionList.Add(cellPos);
			}
		}

		m_emptyCellPositionList.Remove(new Vector2Int(1, 1)); // Player Cell Position
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
		return m_cellSize * new Vector3(a_cellPos.x, a_cellPos.y, 0f);
	}

	public CellData GetCellData(Vector2Int a_cellPos)
	{
		if (a_cellPos.x < 0 || a_cellPos.x >= m_width || a_cellPos.y < 0 || a_cellPos.y >= m_height)
		{
			return null;
		}

		return m_boardData[a_cellPos.x, a_cellPos.y];
	}

	public void SetCellTile(Vector2Int a_cellPos, GroundTile a_tile_prefab)
	{
		CellData newCell;
		CellData prevCell = m_boardData[a_cellPos.x, a_cellPos.y];
		if (prevCell != null)
		{
			prevCell.CleanUp();
			newCell = prevCell;
		}
		else
		{
			newCell = new();
		}

		m_boardData[a_cellPos.x, a_cellPos.y] = newCell;

		var targetCell = m_boardData[a_cellPos.x, a_cellPos.y];

		if (a_tile_prefab)
		{
			GroundTile newTile = Instantiate(a_tile_prefab, m_boardHolder);
			newTile.transform.position = CellPosToWorldPos(a_cellPos);
			targetCell.m_groundTile = newTile;
		}
	}
}
