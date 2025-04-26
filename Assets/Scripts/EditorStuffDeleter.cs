using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class EditorStuffDeleter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        if (name == "spawnerModel")
        {
            Destroy(gameObject);
        }
            
    }
}
