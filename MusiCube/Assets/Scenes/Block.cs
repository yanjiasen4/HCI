using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{
    MagiCube parentSquare;
    int xIdx;
    int yIdx;
    int zIdx;

    public void BindParent(int x, int y, int z)
    {
        parentSquare = transform.parent.GetComponent<MagiCube>();
        xIdx = x;
        yIdx = y;
        zIdx = z;
    }

    void OnTriggerEnter(Collider c)
    {
        if (parentSquare != null)
        {
            parentSquare.ClickSquare(xIdx, yIdx, zIdx);
        }
    }
}
