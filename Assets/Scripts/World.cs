using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    //storage for all chunks based on chunk coords (WorldPos)
    public Dictionary<WorldPos, Chunk> chunks = new Dictionary<WorldPos, Chunk>();
    
    public GameObject chunkPrefab;

    //World Save Stuff
    public string worldName = "world";


    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    //instantiate the chunk at the coords
    //set chunks pos and world variables
    //add it to the chunk dictionary
    public void CreateChunk(int x, int y, int z) {
        //the coords of chunk in world
        WorldPos worldPos = new WorldPos(x, y, z);

        //instantiate chunk at coords using chunk prefab
        GameObject newChunkObject = Instantiate(chunkPrefab, new Vector3(x, y, z), Quaternion.Euler(Vector3.zero)) as GameObject;

        //get the chunk component
        Chunk newChunk = newChunkObject.GetComponent<Chunk>();

        //assign values
        newChunk.pos = worldPos;
        newChunk.world = this;

        //add the chunks to the dictionary with position as the key
        chunks.Add(worldPos, newChunk);

        //if not generate a new chunk
        var terrainGen = new TerrainGen();
        newChunk = terrainGen.ChunkGen(newChunk);
        newChunk.SetBlocksUnmodified();
        bool loaded = Serialization.Load(newChunk);

        //set all chunks generated as unmodifies
        newChunk.SetBlocksUnmodified();
        //then set all the player changes on top of said chunk
        Serialization.Load(newChunk);
    }

    public void DestroyChunk(int x, int y, int z) {
        Chunk chunk = null;

        if (chunks.TryGetValue(new WorldPos(x, y, z), out chunk)) {
            Serialization.SaveChunk(chunk);
            Object.Destroy(chunk.gameObject);
            chunks.Remove(new WorldPos(x, y, z));
        }
    }

    public Chunk GetChunk(int x, int y, int z) {
        WorldPos pos = new WorldPos();
        float multiple = Chunk.chunkSize;

        //gives chunk coords
        pos.x = Mathf.FloorToInt(x / multiple) * Chunk.chunkSize;
        pos.y = Mathf.FloorToInt(y / multiple) * Chunk.chunkSize;
        pos.z = Mathf.FloorToInt(z / multiple) * Chunk.chunkSize;

        //look up chunk in dic and outputs the chunk if one exists at those coords
        Chunk containerChunk = null;
        chunks.TryGetValue(pos, out containerChunk);

        return containerChunk;
    }

    public Block GetBlock(int x, int y, int z) {
        Chunk containerChunk = GetChunk(x, y, z);
        if (containerChunk != null)
        {
            Block block = containerChunk.GetBlock(
                x - containerChunk.pos.x,
                y - containerChunk.pos.y,
                z - containerChunk.pos.z);

            return block;
        }
        else
        {
            return new BlockAir();
        }
    }

    //set block to the proper position in the proper chunk.
    public void SetBlock(int x, int y, int z, Block block) {
        Chunk chunk = GetChunk(x, y, z);

        if (chunk != null) {
            chunk.SetBlock(x - chunk.pos.x, y - chunk.pos.y, z - chunk.pos.z, block);
            chunk.update = true;

            UpdateIfEqual(x - chunk.pos.x, 0, new WorldPos(x - 1, y, z));
            UpdateIfEqual(x - chunk.pos.x, Chunk.chunkSize - 1, new WorldPos(x + 1, y, z));
            UpdateIfEqual(y - chunk.pos.y, 0, new WorldPos(x, y - 1, z));
            UpdateIfEqual(y - chunk.pos.y, Chunk.chunkSize - 1, new WorldPos(x, y + 1, z));
            UpdateIfEqual(z - chunk.pos.z, 0, new WorldPos(x, y, z - 1));
            UpdateIfEqual(z - chunk.pos.z, Chunk.chunkSize - 1, new WorldPos(x, y, z + 1));
        }
    }
    
    //check if value1 = value2 if so update chunk containing position
    //we want all boarder blocks to be updated as well as the affected chunk
    void UpdateIfEqual(int value1, int value2, WorldPos pos)
    {
        if (value1 == value2)
        {
            Chunk chunk = GetChunk(pos.x, pos.y, pos.z);
            if (chunk != null)
                chunk.update = true;
        }
    }
}
