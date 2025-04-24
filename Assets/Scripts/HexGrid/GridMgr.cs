using UnityEngine;

public enum GridType
    {
        SquareDir,
        HexagonDir,
        OctagonDir
    }
public class GridMgr : MonoBehaviour
{
    public static GridMgr Instance { get; private set; }

    [SerializeField] public GridType gridType = GridType.HexagonDir;
    [SerializeReference] public GridBaseData gridData;

    private IGrid _curGrid;
    private IGridFactory _gridFactory;

    private void Awake()
    {
        Instance = this;
        _gridFactory = new GridFactory();
        _curGrid = _gridFactory.CreateGrid(gridType);
    }
    private void Start()
    {
        _curGrid.Init(gridData);
    }

    private void Update()
    {
        bool isSetStartPos = Input.GetMouseButtonUp(0);
        bool isSetGoalPos = Input.GetMouseButtonUp(1);
        if (isSetStartPos || isSetGoalPos)
        {
            RaycastHit hitInfo;
            Ray hit = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(hit, out hitInfo, 100))
            {
                Node hitNode = _curGrid.GetNodeByGameObject(hitInfo.collider.gameObject);
                if (isSetGoalPos && hitNode != null)
                {
                    _curGrid.SetGoalPos(hitNode);
                }
                else
                {
                    _curGrid.SetStartPos(hitNode);
                }
            }
        }
    }
    private void OnValidate()
    {
#if UNITY_EDITOR
        switch (gridType)
        {
            case GridType.SquareDir:
                if (gridData == null || gridData.GetType() != typeof(SquareGridData))
                {
                    gridData = new SquareGridData();
                }
                break;
            case GridType.HexagonDir:
                if (gridData == null || gridData.GetType() != typeof(HexGridData))
                {
                    gridData = new HexGridData();
                }
                break;
            case GridType.OctagonDir:
                break;
            default:
                break;
        }

        UnityEditor.EditorUtility.SetDirty(this); // đảm bảo Inspector cập nhật
#endif
    }
}
