using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[System.Serializable]
public class GenerationElement
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int amount = 1;
    [SerializeField] private Vector2 surfaceAngleMinMax;
    [SerializeField] private Vector2 randomRotationMinMax;
    [SerializeField] private Vector2 randomScaleMinMax;
    [SerializeField] private bool alongBySurfaceNormal;


    public bool TrySpawnPrefab(Transform parent, Vector3 position, Vector3 surfaceNormal, float surfaceAngle)
    {
        if (surfaceAngle < surfaceAngleMinMax.x || surfaceAngle > surfaceAngleMinMax.y) return false;



#if UNITY_EDITOR
        GameObject spawnedObject;

        spawnedObject =  PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        spawnedObject.isStatic = true;

        float randomScale = Random.Range(randomScaleMinMax.x,randomScaleMinMax.y);
        spawnedObject.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        spawnedObject.transform.position = position;

        if (alongBySurfaceNormal == true)
            spawnedObject.transform.up = surfaceNormal;
        
        float randomAngle = Random.Range(randomRotationMinMax.x, randomRotationMinMax.y);

        spawnedObject.transform.RotateAround(spawnedObject.transform.position, spawnedObject.transform.up, randomAngle);
      
        spawnedObject.transform.SetParent(parent);

#endif

        return true;

    }

    public float Amount => amount;
}

public class FoliageGenerator : MonoBehaviour
{
    private const float RaycastHeight = 10000;

    [SerializeField] private GenerationElement[] generationElements;
    [Space(10)] [SerializeField] private Vector2 terrainSize;


    public void Generate()
    {
        DestoryAllChilds();

        for (int i = 0; i < generationElements.Length; i++)
        {
            GenerateElements(generationElements[i]);
        }


    }

    public void DestoryAllChilds()
    {
        Transform[] allChilds = new Transform[transform.childCount];

        for (int i = 0; i < allChilds.Length; i++)
        {
            allChilds[i] = transform.GetChild(i);
        }

        for (int i = 0;  i < allChilds.Length; i++)
        {
            DestroyImmediate(allChilds[i].gameObject);
        }
    }

    // Сделать код чуть более читаемым 
    private void GenerateElements(GenerationElement generationElement)
    {
        int spawnedElementAmount = 0;
        // Переименовать в промахи?
        int generateAttemptAmount = 0;
        int maxGenerateAttempAmount = (int)  generationElement.Amount * 100;

        while (spawnedElementAmount < generationElement.Amount)
        {
            Vector3 randomPoint = GetRandomPointInsideTerrain();

            RaycastHit raycastHit;
            if (Physics.Raycast(randomPoint, Vector3.down, out raycastHit, RaycastHeight))
            {

                if(generationElement.TrySpawnPrefab(transform, raycastHit.point, 
                    raycastHit.normal, Vector3.Angle(raycastHit.normal, Vector3.up)) == true)
                {
                    spawnedElementAmount++;
                }
                else
                {
                    generateAttemptAmount++;
                }
            }

            if(generateAttemptAmount > maxGenerateAttempAmount)
            {
                throw new Exception("Не удалось найти место для генерации элемента с заданными параметрами!");
            }
        }


    }

    private Vector3 GetRandomPointInsideTerrain()
    {
        Vector3 randomPoint = new Vector3();
        randomPoint.x = transform.position.x + Random.Range(-terrainSize.x / 2, terrainSize.x / 2);
        randomPoint.z = transform.position.z + Random.Range(-terrainSize.y / 2, terrainSize.y / 2);
        randomPoint.y = transform.position.y + RaycastHeight;
        return randomPoint;
    }



#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(terrainSize.x, 0, terrainSize.y));
    }


#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(FoliageGenerator))]
public class RoadPointSpawnerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        FoliageGenerator generator = target as FoliageGenerator;

        if (GUILayout.Button("Generate") == true)
        {
            generator.Generate();
            EditorSceneManager.MarkSceneDirty(generator.gameObject.scene);
        }

        if (GUILayout.Button("Destory all childs") == true)
        {
            generator.DestoryAllChilds();
            EditorSceneManager.MarkSceneDirty(generator.gameObject.scene);
        }
    }
}
#endif