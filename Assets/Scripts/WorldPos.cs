using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public struct WorldPos
{
    //for using coords as a key in our dictionary
    public int x, y, z;
    public WorldPos(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    //to make the basic equals function a little faster
    public override bool Equals(object obj)
    {
        if (!(obj is WorldPos))
            return false;
        WorldPos pos = (WorldPos)obj;
        if (pos.x != x || pos.y != y || pos.z != z)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
