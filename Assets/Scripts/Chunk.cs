using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//adds required mesh components automatically
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{
    //array of blocks to store block coords
    public Block[,,] blocks = new Block[chunkSize, chunkSize, chunkSize];
    //size of chunk
    public static int chunkSize = 16;
    //chunk has been changed and will be updated next frame
    public bool update = false;
    //is the chunk rendered yet?
    public bool rendered;

    MeshFilter filter;
    MeshCollider coll;

    //so the chunk has easy access to the world that contains it
    public World world;
    public WorldPos pos;

    void Start(){
        filter = gameObject.GetComponent<MeshFilter>();
        coll = gameObject.GetComponent<MeshCollider>();
    }
    void Update() {
        if (update)
        {
            update = false;
            UpdateChunk();
        }
    }
    
    //return block at certain coords
    public Block GetBlock(int x, int y, int z) {
        //check if coords are within chunk
        if (InRange(x) && InRange(y) && InRange(z))
            return blocks[x, y, z];
        //if not link the world script to find the chunk it is in
        return world.GetBlock(pos.x + x, pos.y + y, pos.z + z);
    }

    //check if block is within chunk
    public static bool InRange(int index) {
        if (index < 0 || index >= chunkSize)
            return false;

        return true;
    }

    //creates a 3d mesh for the chunk based on contents
    void UpdateChunk(){
        MeshData meshData = new MeshData();
        rendered = true;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    meshData = blocks[x, y, z].Blockdata(this, x, y, z, meshData);
                }
            }
        }

        RenderMesh(meshData);
    }

    //sets block in position described by block provided
    //if not in chunk send to the world to get it in the right place
    public void SetBlock(int x, int y, int z, Block block) {
        if (InRange(x) && InRange(y) && InRange(z))
        {
            blocks[x, y, z] = block;
        }
        else {
            //sending to world setblock script
            world.SetBlock(pos.x + x, pos.y + y, pos.z + z, block);
        }
    }
    
    //sends mesh info to the mesh and collider components
    void RenderMesh(MeshData meshData) {
        filter.mesh.Clear();
        filter.mesh.vertices = meshData.vertices.ToArray();
        filter.mesh.triangles = meshData.triangles.ToArray();
        filter.mesh.uv = meshData.uv.ToArray();
        filter.mesh.RecalculateNormals();

        //remove current collider mesh and creates new mesh to apply the verts and tris
        //then use new mesh for collision mesh
        coll.sharedMesh = null;
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.colVertices.ToArray();
        mesh.triangles = meshData.colTriangles.ToArray();
        mesh.RecalculateNormals();

        coll.sharedMesh = mesh;
    }

    //set the blocks to unchanged by player
    public void SetBlocksUnmodified()
    {
        foreach (Block block in blocks)
        {
            block.changed = false;
        }
    }
}
