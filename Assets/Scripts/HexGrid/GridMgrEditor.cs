#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridMgr))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridMgr mgr = (GridMgr)target;

        // Enum field to choose GridType (A, B, or C)
        mgr.gridType = (GridType)EditorGUILayout.EnumPopup("Grid Type", mgr.gridType);

        // Display the appropriate fields based on selected GridType
        switch (mgr.gridType)
        {
            case GridType.SquareDir:
                if (mgr.gridData == null || !(mgr.gridData is GridA))
                {
                    mgr.gridData = new GridA(); // Create new GridA if not assigned
                }
                DrawGridA((GridA)mgr.gridData);
                break;

            case GridType.GridB:
                if (mgr.gridData == null || !(mgr.gridData is GridB))
                {
                    mgr.gridData = new GridB(); // Create new GridB if not assigned
                }
                DrawGridB((GridB)mgr.gridData);
                break;

            case GridType.GridC:
                if (mgr.gridData == null || !(mgr.gridData is GridC))
                {
                    mgr.gridData = new GridC(); // Create new GridC if not assigned
                }
                DrawGridC((GridC)mgr.gridData);
                break;
        }

        // Apply changes to serialized object
        serializedObject.ApplyModifiedProperties();
    }

    // Helper methods to draw fields for each grid type
    private void DrawGridA(GridA gridA)
    {
        gridA.a1 = EditorGUILayout.IntField("A1", gridA.a1);
        gridA.a2 = EditorGUILayout.IntField("A2", gridA.a2);
        gridA.a3 = EditorGUILayout.IntField("A3", gridA.a3);
        gridA.a4 = EditorGUILayout.IntField("A4", gridA.a4);
    }

    private void DrawGridB(GridB gridB)
    {
        gridB.b1 = EditorGUILayout.IntField("B1", gridB.b1);
        gridB.b2 = EditorGUILayout.IntField("B2", gridB.b2);
        gridB.b3 = EditorGUILayout.IntField("B3", gridB.b3);
        gridB.b4 = EditorGUILayout.IntField("B4", gridB.b4);
        gridB.b5 = EditorGUILayout.IntField("B5", gridB.b5);
        gridB.b6 = EditorGUILayout.IntField("B6", gridB.b6);
    }

    private void DrawGridC(GridC gridC)
    {
        gridC.c1 = EditorGUILayout.IntField("C1", gridC.c1);
        gridC.c2 = EditorGUILayout.IntField("C2", gridC.c2);
        gridC.c3 = EditorGUILayout.IntField("C3", gridC.c3);
        gridC.c4 = EditorGUILayout.IntField("C4", gridC.c4);
        gridC.c5 = EditorGUILayout.IntField("C5", gridC.c5);
        gridC.c6 = EditorGUILayout.IntField("C6", gridC.c6);
        gridC.c7 = EditorGUILayout.IntField("C7", gridC.c7);
        gridC.c8 = EditorGUILayout.IntField("C8", gridC.c8);
    }
}
#endif
