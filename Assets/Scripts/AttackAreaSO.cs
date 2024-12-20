using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Area", menuName = "Attack Area")]
public class AttackAreaSO : ScriptableObject
{
    public const int GridSize = 9; // Fixed size, ODD NUMBER.
    public const int Center = GridSize / 2; // Player's position in the grid (7, 7)
    public List<bool> shape = new List<bool>(GridSize * GridSize); // Flattened grid

    private void OnEnable()
    {
        // Ensure the grid is properly initialized
        if (shape == null || shape.Count != GridSize * GridSize)
        {
            shape = new List<bool>(new bool[GridSize * GridSize]);
        }
    }

    public bool GetCell(int x, int y)
    {
        return shape[y * GridSize + x];
    }

    public void SetCell(int x, int y, bool value)
    {
        shape[y * GridSize + x] = value;
    }
}
