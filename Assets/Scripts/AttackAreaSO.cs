using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Area", menuName = "Attack Area")]
public class AttackAreaSO : ScriptableObject
{
    public const int GridSize = 9; // Fixed size, ODD NUMBER.
    public const int Center = GridSize / 2;
    public bool[,] shape = new bool[GridSize, GridSize]; // Fixed-size 2D array
}
