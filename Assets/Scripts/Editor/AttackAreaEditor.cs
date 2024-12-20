using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AttackAreaSO))]
public class AttackAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var attackArea = (AttackAreaSO)target;
        bool gridChanged = false; // Track changes to the grid

        // Draw the fixed grid
        for (int y = 0; y < AttackAreaSO.GridSize; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < AttackAreaSO.GridSize; x++)
            {
                if (x == AttackAreaSO.Center && y == AttackAreaSO.Center)
                {
                    // Gray out the center cell (player's position)
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.Toggle(true, GUILayout.Width(20));
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    // Editable cells
                    bool currentValue = attackArea.GetCell(x, y);
                    bool newValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(20));
                    if (newValue != currentValue)
                    {
                        attackArea.SetCell(x, y, newValue);
                        gridChanged = true;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        // Mark as dirty and save changes if needed
        if (gridChanged)
        {
            EditorUtility.SetDirty(attackArea);
        }
    }
}
