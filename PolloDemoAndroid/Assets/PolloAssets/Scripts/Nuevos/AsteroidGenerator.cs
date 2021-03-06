﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGenerator : MonoBehaviour {

    public static AsteroidGenerator AsteroidGeneratorSingelton = null;          //instancia para el geneardor de meteoros
    [SerializeField]
    private Transform Target;
    public Vector3 Offset;
    public GameObject AsteroidPrefab = null;
    public int AsteroidPoolSize = 20;

    public float TimeToStartAsteroidWave = 2f;
    public float SpeedAsteroid = 5f;
    public float SpeedAsteroidWave = 1f;

    public bool EnableAsteroidWave;
    private GameObject[] asteroidArray;

    public Queue<Transform> AsteroidQueue = new Queue<Transform>();

    private void Awake()
    {
        if (AsteroidGeneratorSingelton != null) {
            Destroy(gameObject);
            return;
        }

        AsteroidGeneratorSingelton = this;
    }
    void Start () {
        Target = CharacterManager.characterManagerInstance.character.transform;
        CreateAsteroids();
        StartCoroutine(CorStartAsteroidWave());
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 desiredPosition = new Vector3(transform.position.x, transform.position.y, Target.position.z) + Offset;
        transform.position = desiredPosition;
    }

    void CreateAsteroids() {

        asteroidArray = new GameObject[AsteroidPoolSize];

        for (int i = 0; i < AsteroidPoolSize; i++)
        {
            asteroidArray[i] = Instantiate(AsteroidPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            Transform objTrans = asteroidArray[i].GetComponent<Transform>();
            //objTrans.parent = transform;
            objTrans.gameObject.name = objTrans.gameObject.name + "_" + i;
            AsteroidQueue.Enqueue(objTrans);
            if (asteroidArray[i].activeInHierarchy)
                asteroidArray[i].SetActive(false);
        }
    }

    public static Transform SpawnAsteroid(Vector3 position, Quaternion rotation)
    {
        Transform spawnedAsteroid = AsteroidGeneratorSingelton.AsteroidQueue.Dequeue();

        spawnedAsteroid.gameObject.SetActive(true);
        spawnedAsteroid.position = position;
        spawnedAsteroid.rotation = rotation;

        AsteroidGeneratorSingelton.AsteroidQueue.Enqueue(spawnedAsteroid);

        return spawnedAsteroid;
    }

    IEnumerator CorStartAsteroidWave() {
        yield return new WaitForSeconds(TimeToStartAsteroidWave);
        while (EnableAsteroidWave) {
            Vector3 posLeft = new Vector3(-5f, 2f, transform.position.z);
            Transform tra = AsteroidGenerator.SpawnAsteroid(posLeft, Quaternion.Euler(0f, 180f, 0f));
            tra.GetComponent<Asteroid>().speedForward = SpeedAsteroid;
            tra.gameObject.SetActive(true);

            yield return new WaitForSeconds(SpeedAsteroidWave);

            Vector3 posCenter = new Vector3(0f, 0f, transform.position.z);
            Transform tra1 = AsteroidGenerator.SpawnAsteroid(posCenter, Quaternion.Euler(0f, 180f, 0f));
            tra1.GetComponent<Asteroid>().speedForward = SpeedAsteroid;
            tra1.gameObject.SetActive(true);

            yield return new WaitForSeconds(SpeedAsteroidWave);

            Vector3 posRight = new Vector3(5f, -2f, transform.position.z);
            Transform tra2 = AsteroidGenerator.SpawnAsteroid(posRight, Quaternion.Euler(0f, 180f, 0f));
            tra2.GetComponent<Asteroid>().speedForward = SpeedAsteroid;
            tra2.gameObject.SetActive(true);

            yield return new WaitForSeconds(SpeedAsteroidWave);

        }
    }

}
