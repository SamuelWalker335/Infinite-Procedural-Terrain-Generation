using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainV
{
    //Round vector3 coords to the nearest block position
    public static WorldPos GetBlockPos(Vector3 pos) {
        WorldPos blockPos = new WorldPos(
            Mathf.RoundToInt(pos.x),
            Mathf.RoundToInt(pos.y),
            Mathf.RoundToInt(pos.z)
            );
        return blockPos;
    }
    //get position from raycast collision
    public static WorldPos GetBlockPos(RaycastHit hit, bool adjacent = false)
    {
        Vector3 pos = new Vector3(
            MoveWithinBlock(hit.point.x, hit.normal.x, adjacent),
            MoveWithinBlock(hit.point.y, hit.normal.y, adjacent),
            MoveWithinBlock(hit.point.z, hit.normal.z, adjacent)
            );

        return GetBlockPos(pos);
    }

    static float MoveWithinBlock(float pos, float norm, bool adjacent = false) {
        if (pos - (int)pos == 0.5f || pos - (int)pos == -0.5f) {
            if (adjacent)
            {
                pos += (norm / 2);
            }
            else {
                pos -= (norm / 2);
            }
        }
        return (float)pos;
    }

    //takes a raycastHit and gets the chunk hit if no chunk return false
    //otherwise get position of the block hit and call set block on chunks world comp
    public static bool SetBlock(RaycastHit hit, Block block, bool adjacent = false) {
        Chunk chunk = hit.collider.GetComponent<Chunk>();
        if (chunk == null)
            return false;

        WorldPos pos = GetBlockPos(hit, adjacent);

        chunk.world.SetBlock(pos.x, pos.y, pos.z, block);

        return true;
    }
    //same as set block but now we return the hit block
    public static Block GetBlock(RaycastHit hit, bool adjacent = false) {
        Chunk chunk = hit.collider.GetComponent<Chunk>();
        if (chunk == null)
            return null;

        WorldPos pos = GetBlockPos(hit, adjacent);

        Block block = chunk.world.GetBlock(pos.x, pos.y, pos.z);

        return block;
    }
}
