using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MapLoader : MonoBehaviour
{
    // Get path to JSON file in Unity
    [SerializeField]
    private string mapPath = null;

    // Array of map objects
    private MapObjectCollection mapObjectCollection;
    //private static GameObject _player;

    [ContextMenu("Load Map from File")]
    private void LoadMap()
    {
        if (mapPath != null)
        {
            // Reads from JSON file and loads into Unity
            using (StreamReader stream = new StreamReader(mapPath))
            {
                string json = stream.ReadToEnd();
                mapObjectCollection = JsonUtility.FromJson<MapObjectCollection>(json);
            }

            if (mapObjectCollection.mapObjects == null)
            {
                Debug.Log("ERROR: JSON map is empty.");
            }
            else
            {
                for (int i = 0; i < mapObjectCollection.mapObjects.Length; ++i)
                {
                    if (mapObjectCollection.mapObjects[i].objectName.Contains("floor"))
                    {
                        GameObject obj = mapObjectCollection.mapObjects[i]
                                        .CreateFloor();
                        //mapObjectCollection.floor = obj.transform;
                    }
                    else if (mapObjectCollection.mapObjects[i].objectName.Contains("border"))
                    {
                        GameObject obj = mapObjectCollection.mapObjects[i]
                                        .CreateBorder();
                    }
                    else if (mapObjectCollection.mapObjects[i].objectName.Contains("wall"))
                    {
                        GameObject obj = mapObjectCollection.mapObjects[i]
                                        .CreateWall();
                    }
                    else if (mapObjectCollection.mapObjects[i].objectName.Contains("player"))
                    {
                        GameObject obj = mapObjectCollection.mapObjects[i]
                                        .CreatePlayer();
                        //mapObjectCollection.player = obj;
                    }
                    else if (mapObjectCollection.mapObjects[i].objectName.Contains("game_manager"))
                    {
                        GameObject obj = mapObjectCollection.mapObjects[i]
                                        .CreateGameManager();
                    }
                    else if (mapObjectCollection.mapObjects[i].objectName.Contains("bush"))
                    {
                        GameObject obj = mapObjectCollection.mapObjects[i]
                                        .CreateBush();
                    }
                    else
                    {
                        GameObject obj = mapObjectCollection.mapObjects[i]
                                        .CreateEmpty();
                    }
                }

                Debug.Log("Map Loaded!");
            }
        }
        else
        {
            Debug.Log("ERROR: map path is not defined.");
        }
    }
    /*
    [ContextMenu("Write Map to File")]
    private void SaveMap()
    {
        GenerateMap();

        using (StreamWriter stream = new StreamWriter(mapPath))
        {
            string json = JsonUtility.ToJson(mapPath);
            stream.Write(json);
        }

        Debug.Log("Map Saved to " + mapPath);
    }

    // Generates the map needed to write to JSON
    private void GenerateMap()
    {
        MapObject[] sampleMapObjects = new MapObject[1];
        sampleMapObjects[0] = new Wall(0);

        mapObjectCollection = new MapObjectCollection()
            { mapObjects = sampleMapObjects, mapName = "map_test" };
    }
    //*/
}

[Serializable]
public class MapObjectCollection
{
    public MapObject[] mapObjects;
    //public GameObject player;
    //public Transform floor;
}

[Serializable]
public class MapObject
{
    // Name of the object
    public string objectName;

    // Position of the object
    public float t_positionx;
    public float t_positiony;
    public float t_positionz;
    Vector3 t_position() { return new Vector3(t_positionx, t_positiony, t_positionz); }

    // Size of the object
    public float t_scalex;
    public float t_scaley;
    public float t_scalez;
    Vector3 t_scale() { return new Vector3(t_scalex, t_scaley, t_scalez); }

    // Base Class
    public MapObject()
    {
        objectName = "test";
        t_positionx = t_positiony = t_positionz = 0f;
        t_scalex = t_scaley = t_scalez = 1f;
    }
    // Returns an empty object with given name
    public GameObject CreateEmpty()
    {
        GameObject ret = new GameObject();
        ret.name = this.objectName;
        return ret;
    }
    //*
    // Returns an object with player manager attached to it
    // FIXME: doesnt automatically add player
    public GameObject CreateGameManager()
    {
        GameObject gm = new GameObject();
        gm.name = this.objectName;
        gm.AddComponent<PlayerManager>();
        PlayerManager pm = gm.GetComponent<PlayerManager>();
        //pm.player = playerobj;
        Debug.Log("Add 'player' object to this object " + gm.name);
        return gm;
    }
    //*/
    //*
    // Returns a Player object with all the necessary properties
    // FIXME: does not get automatically added to game manager
    public GameObject CreatePlayer()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = this.objectName;
        player.transform.localScale = new Vector3(1f, 1f, 1f);
        player.transform.position = new Vector3(0f, t_positiony, t_positionz);
        player.GetComponent<MeshRenderer>().material =
            Resources.Load("Materials/Player_Mat") as Material;
        player.AddComponent<CharacterController>();
        player.AddComponent<PlayerController>();
        PlayerController pc = player.GetComponent<PlayerController>();
        pc.controller = player.GetComponent<CharacterController>();
        player.layer = 10; // 10 = Player

        GameObject face = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        face.name = "face";
        face.transform.localScale = new Vector3(.75f,.25f,.25f);
        face.transform.parent = player.transform;
        face.transform.localPosition = new Vector3(0f, .5f, .25f);
        face.transform.Rotate(new Vector3(90f,0f,0f), Space.Self);

        GameObject focus = new GameObject();
        focus.transform.parent = player.transform;
        focus.transform.localPosition = new Vector3(
            0f,
            (.2f + (player.transform.localScale.y)),
            0f);
        focus.name = "cam_focus";

        Debug.Log("Add 'main camera' to this object " + player.name);
        Debug.Log("Add this object '" + player.name 
            + "' to 'CineMachineFreelook.Follow' and the object '" 
            + player.name + "." + focus.name + "' tp "
            + "'CineMachineFreelook.LookAt'");
        return player;
    }
    //*/
    // Returns a Floor object with all the necessary properties
    public GameObject CreateFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = this.objectName;
        floor.GetComponent<MeshRenderer>().material =
            Resources.Load("Materials/Ground_Mat") as Material;
        floor.transform.localScale = t_scale();
        floor.transform.position = t_position();
        floor.layer = 9; // 9 = Environment
        floor.isStatic = true;
        return floor;
    }
    // Returns a Border object with all the necessary properties
    public GameObject CreateBorder()
    {
        GameObject border = GameObject.CreatePrimitive(PrimitiveType.Cube);
        border.name = this.objectName;
        border.GetComponent<MeshRenderer>().material =
            Resources.Load("Materials/Walls_Mat") as Material;
        border.transform.localScale = new Vector3(t_scalex, 6f, t_scalez);
        border.transform.position = new Vector3(t_positionx, 2f, t_positionz);
        border.layer = 9; // 9 = Environment
        border.isStatic = true;
        return border;
    }
    // Returns a Wall object with all the necessary properties
    public GameObject CreateWall()
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = this.objectName;
        wall.GetComponent<MeshRenderer>().material = 
            Resources.Load("Materials/Walls_Mat") as Material;
        wall.transform.localScale = new Vector3(t_scalex, 3f, t_scalez);
        wall.transform.position = new Vector3(t_positionx, 0f, t_positionz);
        wall.layer = 9; // 9 = Environment
        wall.isStatic = true;
        return wall;
    }
    // Returns a Bush object with all the necessary properties
    public GameObject CreateBush()
    {
        GameObject bush = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bush.name = this.objectName;
        bush.GetComponent<MeshRenderer>().material =
            Resources.Load("Materials/Vegetation_Mat") as Material;
        bush.AddComponent<EnvironmentController>();
        bush.transform.localScale = new Vector3(3f, 3.5f, 3f);
        bush.transform.position = new Vector3(t_positionx, 0f, t_positionz);
        bush.layer = 9; // 9 = Environment
        //bush.isStatic = true;
        return bush;
    }
}
