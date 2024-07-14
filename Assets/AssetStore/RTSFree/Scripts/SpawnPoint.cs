using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;

namespace RTSToolkitFree
{
    public class SpawnPoint : MonoBehaviour
    {
        public UnitPars genericUnit;
        public Unit[] unitData;
        public bool randomizeRotation = true;
        public int numberOfObjects = 10000;
        public float timestep = 0.01f;
        public float size = 1.0f;
        public Vector3 posOffset;

        private Terrain ter;
        private float spawnTimer;
        private TMP_Text text;
        
        private void Start()
        {
            ter = FindObjectOfType<Terrain>();
        }

        private void Update()
        {
            Spawn();
        }

        public void Init(Building b, List<Unit> u)
        {
            text = transform.GetChild(0).GetComponentInChildren<TMP_Text>();
            
            if (text == null)
            {
                Debug.Log("No text found in children !");
                return;
            }

            var unitPars = GetComponent<UnitPars>();

            name = b.name;
            
            unitPars.desc = b.description;
            unitPars.health = unitPars.maxHealth = b.maxHealth;
            unitPars.defence = b.defence;
            timestep = b.spawnDelay;
            
            text.text = b.visualData.fallbackAsciiArt;

            unitData = u.ToArray();
        }
        
        private void Spawn()
        {
            if (numberOfObjects <= 0)
            {
                return;
            }

            spawnTimer -= Time.deltaTime;
            if (spawnTimer > 0f)
            {
                return;
            }

            UnitPars spawnPointUp = GetComponent<UnitPars>();
            if(spawnPointUp != null)
            {
                if(spawnPointUp.health <= 0)
                {
                    numberOfObjects = 0;
                    return;
                }
            }

            Quaternion rot = transform.rotation;
            if (randomizeRotation)
            {
                rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            }

            Vector2 randPos = Random.insideUnitCircle * size;

            Vector3 pos = transform.position + new Vector3(randPos.x, 0f, randPos.y) + transform.rotation * posOffset;
            pos = TerrainVector(pos, ter);

            // Choisir data random à spawn
            UnitPars instance = Instantiate(genericUnit, pos, rot);
            instance.Init(unitData[Random.Range(0,unitData.Length)]);
            UnitPars instanceUp = instance.GetComponent<UnitPars>();

            if (instanceUp != null)
            {
                if(instanceUp.nation >= Diplomacy.active.numberNations)
                {
                    Diplomacy.active.AddNation();
                }

                instanceUp.isReady = true;

                if (instanceUp.changeMaterial)
                {
                    instanceUp.GetComponent<Renderer>().material.color = Color.yellow;
                }
            }

            BattleSystem.active.allUnits.Add(instanceUp);

            numberOfObjects--;
            spawnTimer = timestep;
        }

        Vector3 TerrainVector(Vector3 origin, Terrain ter1)
        {
            if (ter1 == null)
            {
                return origin;
            }

            Vector3 planeVect = new Vector3(origin.x, 0f, origin.z);
            float y = ter1.SampleHeight(planeVect);

            y = y + ter1.transform.position.y;

            Vector3 tv = new Vector3(origin.x, y, origin.z);
            return tv;
        }
    }
}
