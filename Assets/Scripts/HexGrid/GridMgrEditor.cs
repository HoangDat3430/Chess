#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(GridMgr))]
public class GridManagerEditor : Editor
{
    [NonSerialized] public Dictionary<GridType, GridBaseData> savedData = new();
    public override void OnInspectorGUI()
    {
        var mgr = (GridMgr)target;
        var prevType = mgr.gridType;

        // Dropdown
        GridType newType = (GridType)EditorGUILayout.EnumPopup("Grid Type", mgr.gridType);

        // Khởi tạo dictionary nếu null
        if (savedData == null)
        {
            savedData = new Dictionary<GridType, GridBaseData>();
        }

        // Nếu đổi type
        if (newType != prevType)
        {
            // Lưu lại current gridData nếu chưa có trong savedData
            if (mgr.gridData != null && !savedData.ContainsKey(prevType))
            {
                savedData[prevType] = mgr.gridData;
            }

            // Cập nhật type
            mgr.gridType = newType;

            // Load lại data nếu đã có trước đó
            if (savedData.TryGetValue(newType, out var saved))
            {
                mgr.gridData = saved;
            }
            else
            {
                // Tạo mới nếu chưa có
                mgr.gridData = CreateGridData(newType);
            }

            // Đánh dấu object đã thay đổi để Unity lưu lại
            EditorUtility.SetDirty(mgr);
        }

        // Hiển thị thuộc tính cụ thể của type
        if (mgr.gridData != null)
        {
            SerializedProperty gridDataProp = serializedObject.FindProperty("gridData"); // Get the reference to field named "gridData" inside target object (GridMgr) 
            EditorGUILayout.PropertyField(gridDataProp, true);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private GridBaseData CreateGridData(GridType type)
    {
        return type switch
        {
            GridType.Square => new SquareGridData(),
            GridType.Hexagon => new HexGridData(),
            _ => null
        };
    }
}
#endif
