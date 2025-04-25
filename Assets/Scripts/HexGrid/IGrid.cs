using UnityEngine;

public interface IGrid
{
    public void SetStartPos(Node node);
    public void SetGoalPos(Node node);
    public void Init(GridBaseData data);
    public Vector3 GetCenter(int x, int y);
    public Node GetNodeByGameObject(GameObject go);
}
