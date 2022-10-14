using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public Transform buildablesParent;
    static Transform parent;
    public PlayerBuildManager _buildManager;
    static PlayerBuildManager buildManager;

    private void Start()
    {
        parent = buildablesParent;
        buildManager = _buildManager;
    }

    private void Update()
    {
        if (Input.GetButtonDown("save"))
        {
            string saved = Save();
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
            Load(saved);
        }
    }

    public static string Save()
    {
        string data = "";

        foreach (Transform child in parent)
        {
            data += $"{child.GetComponent<BuiltBuildable>().buildable.name}~{child.position.x}~{child.position.y}~{child.position.z}~{child.rotation.eulerAngles.x}~{child.rotation.eulerAngles.y}~{child.rotation.eulerAngles.z}/";
        }

        return data;
    }

    public static void Load(string data)
    {
        string currentObjectString = "";

        foreach (char _char in data)
        {
            string character = _char.ToString();
            if (character == "/")
            {
                string name = "";
                foreach (char __char in currentObjectString)
                {
                    string _character = __char.ToString();
                    if (_character == "~")
                    {
                        break;
                    } else
                    {
                        name += _character;
                    }
                } // Get name of object
                currentObjectString = currentObjectString.Substring(name.Length+1);

                // Get Position of object in string
                float x = 0f;
                float y = 0f;
                float z = 0f;

                string _x = "";
                string _y = "";
                string _z = "";

                foreach (char __char in currentObjectString)
                {
                    string _character = __char.ToString();
                    if (_character == "~")
                    {
                        break;
                    } else
                    {
                        _x += _character;
                    }
                } // Get X
                currentObjectString = currentObjectString.Substring(_x.Length + 1);
                foreach (char __char in currentObjectString)
                {
                    string _character = __char.ToString();
                    if (_character == "~")
                    {
                        break;
                    }
                    else
                    {
                        _y += _character;
                    }
                } // Get Y
                currentObjectString = currentObjectString.Substring(_y.Length + 1);
                foreach (char __char in currentObjectString)
                {
                    string _character = __char.ToString();
                    if (_character == "~")
                    {
                        break;
                    }
                    else
                    {
                        _z += _character;
                    }
                } // Get Z
                currentObjectString = currentObjectString.Substring(_z.Length + 1);

                // Get Position of object in string
                float rotX = 0f;
                float rotY = 0f;
                float rotZ = 0f;

                string _rotX = "";
                string _rotY = "";
                string _rotZ = "";

                foreach (char __char in currentObjectString)
                {
                    string _character = __char.ToString();
                    if (_character == "~")
                    {
                        break;
                    }
                    else
                    {
                        _rotX += _character;
                    }
                } // Get X
                currentObjectString = currentObjectString.Substring(_rotX.Length + 1);
                foreach (char __char in currentObjectString)
                {
                    string _character = __char.ToString();
                    if (_character == "~")
                    {
                        break;
                    }
                    else
                    {
                        _rotY += _character;
                    }
                } // Get Y
                currentObjectString = currentObjectString.Substring(_rotY.Length + 1);
                foreach (char __char in currentObjectString)
                {
                    string _character = __char.ToString();
                    if (_character == "/")
                    {
                        break;
                    }
                    else
                    {
                        _rotZ += _character;
                    }
                } // Get Z

                // Convert to float, then Vector3
                x = float.Parse(_x);
                y = float.Parse(_y);
                z = float.Parse(_z);
                Vector3 pos = new Vector3(x, y, z);

                // Convert to float, then Vector3
                rotX = float.Parse(_rotX);
                rotY = float.Parse(_rotY);
                rotZ = float.Parse(_rotZ);
                Vector3 rot = new Vector3(rotX, rotY, rotZ);

                Vector3Int gridPosition = new Vector3Int(
                Mathf.RoundToInt(buildManager.buildPoint.x / buildManager.gridSize.x + buildManager.buildablesGridSize.x / 2),
                Mathf.RoundToInt(buildManager.buildPoint.y / buildManager.gridSize.y + buildManager.buildablesGridSize.y / 2),
                Mathf.RoundToInt(buildManager.buildPoint.z / buildManager.gridSize.z + buildManager.buildablesGridSize.z / 2));

                buildManager.grid[gridPosition.x, gridPosition.y, gridPosition.z] = Instantiate(buildManager.GetBuildableWithName(name).buildPrefab, pos, Quaternion.Euler(rot), parent);
                buildManager.grid[gridPosition.x, gridPosition.y, gridPosition.z].GetComponent<BuiltBuildable>().gridIndicies = gridPosition;

                currentObjectString = "";
            } else
            {
                currentObjectString += character;
            }
        }
    }

}
