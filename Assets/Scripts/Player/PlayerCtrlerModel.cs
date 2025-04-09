using System.Collections;
using System.Collections.Generic;

public class PlayerCtrlerModel
{
    public bool isMovable { get; private set; } = true;
    public bool isRotatable { get; private set; } = true;

    public void SetMovable(bool x)
    {
        isMovable = x;
    }

    public void SetRotetable(bool x)
    {
        isRotatable = x;
    }

}
