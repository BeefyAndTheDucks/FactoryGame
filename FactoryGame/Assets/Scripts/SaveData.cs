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
            data += $"{child.GetComponent<BuiltBuildable>().buildable.name},{child.position.x}.{child.position.y}~{child.position.z}/";
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
                    if (_character == ",")
                    {
                        break;
                    } else
                    {
                        name += _character;
                    }
                } // Get name of object
                currentObjectString = currentObjectString.Substring(name.Length+1);

                // Get Position of object in string
                float x = 0.0f;
                float y = 0.0f;
                float z = 0.0f;

                string _x = "";
                string _y = "";
                string _z = "";

                foreach (char __char in currentObjectString)
                {
                    string _character = __char.ToString();
                    if (_character == ".")
                    {
                        break;
                    } else
                    {
                        _x += _character;
                    }
                }
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
                }
                currentObjectString = currentObjectString.Substring(_y.Length + 1);
                foreach (char __char in currentObjectString)
                {
                    _z += __char.ToString();
                }

                // Convert to float, then Vector3
                x = float.Parse(_x);
                y = float.Parse(_y);
                z = float.Parse(_z);
                Vector3 pos = new Vector3(x, y, z);

                Instantiate(buildManager.GetBuildableWithName(name).buildPrefab, pos, Quaternion.identity, parent);

                currentObjectString = "";
            } else
            {
                currentObjectString += character;
            }
        }
    }

}
