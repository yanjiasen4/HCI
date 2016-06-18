using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{
    GameManager gameManager;
    public int xIdx;
    public int yIdx;
    public int zIdx;

    public void BindParent(int x, int y, int z)
    {
        gameManager = transform.parent.parent.GetComponent<GameManager>();
        xIdx = x;
        yIdx = y;
        zIdx = z; 
    }

    void OnTriggerEnter(Collider c)
    {
        if (gameManager != null)
        {
            print(xIdx + " " + yIdx + " " + zIdx);
            gameManager.ClickSquare(xIdx, yIdx, zIdx);
        }
    }
}
