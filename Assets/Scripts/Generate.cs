using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generate : MonoBehaviour
{
    //Chunk Properties
    public GameObject Cube;
    public GameObject Chunk;
    public int ChunkSize = 16;
    private Vector3 Origin = new Vector3(0, 0, 0);

    //When To generate Chunk
    public GameObject Player;
    public float viewDistance = 4;

   
    // Start is called before the first frame update
    void Start()
    {
        for (int x = -1; x <= 1; x++){
            for (int z = -1; z <= 1; z++) {
                createChunk(new Vector3(Player.transform.position.x/(ChunkSize + x), 0, Player.transform.position.z/(ChunkSize+z)));
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
  
    }

    //creates a cube
    void CreateCube(Vector3 Location, Transform parent) {
        Instantiate(Cube, Location, Quaternion.identity.normalized, parent);
    }

    //generate a chunk
    void createChunk(Vector3 Location) {
        GameObject TerrainChunk = Instantiate(Chunk, Location, Quaternion.identity.normalized);
        for (int x = -ChunkSize; x <= ChunkSize; x++) {
            for (int y = -ChunkSize; y <= ChunkSize; y++) {
                for (int z = -ChunkSize; z <= ChunkSize; z++) {
                    CreateCube(Location + new Vector3(x, y, z),TerrainChunk.transform);
                }
            }
            
        }
    }
}
