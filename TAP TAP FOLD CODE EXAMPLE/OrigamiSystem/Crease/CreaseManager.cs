using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class CreaseManager
{
    public List<CreasePosition> creases;

    public GameObject creasePrefab;

    public List<GameObject> creaseObjects;

    public void GenerateCreases(int step)
    {
        if (creaseObjects == null && step < creases.Count) // no crease at all
        {
            for (int i = 0; i < creases[step].creases.Count; i++)
            {
                GameObject crease = GameObject.Instantiate(creasePrefab, Vector3.zero, Quaternion.identity);

                crease.GetComponent<LineRenderer>().SetPositions(creases[step].creases[i].points);

                creaseObjects.Add(crease);
            }
        }
        else
        {
            for(int i = 0; i < creaseObjects.Count && i < creases[step].creases.Count; i++) // if there are some creases
            {
                creaseObjects[i].GetComponent<LineRenderer>().SetPositions(creases[step].creases[i].points);
            }

            for (int i = creaseObjects.Count; i < creases[step].creases.Count; i++)
            {
                GameObject crease = GameObject.Instantiate(creasePrefab, Vector3.zero, Quaternion.identity);

                crease.GetComponent<LineRenderer>().SetPositions(creases[step].creases[i].points);

                creaseObjects.Add(crease);
            }

            if (creaseObjects.Count > creases[step].creases.Count)
            {
                for (int i = creases[step].creases.Count; i< creaseObjects.Count;i++ )
                {
                    creaseObjects[i].SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < creaseObjects.Count; i++)
                {
                    creaseObjects[i].SetActive(true);
                }
            }
        }
    }

    public void HideCreases()
    {
        foreach (GameObject creaseObject in creaseObjects)
        {
            creaseObject.SetActive(false);
        }
    }
}
