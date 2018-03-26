using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Source: https://answers.unity.com/questions/487121/automatically-assigning-gameobjects-a-unique-and-c.html

[CustomEditor(typeof(SaveState))]
public class UniqueIdentifierDrawer : Editor
{
    void OnEnable()
    {
        SaveState id = ((SaveState)target);

        // generate new guid when you create a new game object
        if (id.uniqueID == "") id.uniqueID = Guid.NewGuid().ToString();
        else
        {
            SaveState[] objects = Array.ConvertAll(GameObject.FindObjectsOfType(typeof(SaveState)), x => x as SaveState);
            int idCount = 0;
            for (int i = 0; i < objects.Length; i++)
            {
                if (id.uniqueID == objects[i].uniqueID)
                    idCount++;
            }
            if (idCount > 1) id.uniqueID = Guid.NewGuid().ToString();
        }
    }
}
