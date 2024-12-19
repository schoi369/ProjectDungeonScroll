using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AttackAreaSO))]
public class AttackAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var attackArea = (AttackAreaSO)target;

        // Draw the fixed 15x15 grid
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
                    // Normal toggle for other cells
                    attackArea.shape[x, y] = EditorGUILayout.Toggle(attackArea.shape[x, y], GUILayout.Width(20));
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        // Apply changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(attackArea);
        }
    }
}
