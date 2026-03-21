using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ChunkObject", order = 1)]
public class ChunkObject : ScriptableObject
{

    public GameManager.ObstacleTypes[] topFloorLayout;
    public GameManager.ObstacleTypes[] midTopFloorLayout;
    public GameManager.ObstacleTypes[] midBottomFloorLayout;
    public GameManager.ObstacleTypes[] bottomFloorLayout;
}