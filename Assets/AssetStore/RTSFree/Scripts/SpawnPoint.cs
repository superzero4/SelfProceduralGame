using System;
using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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
        private Team team;
        
        private void Start()
        {
            ter = FindObjectOfType<Terrain>();
        }

        private void Update()
        {
            Spawn();
        }

        public void Init(Building b, List<Unit> u, Team t)
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
            unitPars.nation = (int)t;
            timestep = b.spawnDelay;
            
            text.text = b.visualData.fallbackAsciiArt;
            text.color = GetTeamColor(t);
            team = t;
            
            if (u != null)
            {
                unitData = u.ToArray();
            }
        }
        
        private void Spawn()
        {
            if(unitData == null || unitData.Length == 0) return;
            
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
            instance.Init(unitData[Random.Range(0,unitData.Length)], team);
            UnitPars instanceUp = instance.GetComponent<UnitPars>();

            if (instanceUp != null)
            {
                if(instanceUp.nation >= Diplomacy.active.numberNations)
                {
                    Diplomacy.active.AddNation();
                }

                instanceUp.isReady = true;
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

        public static Color GetTeamColor(Team teamIndex)
        {
            switch (teamIndex)
            {
                case Team.Red:
                    return new Color(0.8867924f, 0.1213065f, 0.3441888f, 1);
                case Team.Blue:
                    return new Color(0.4377504f, 0.1840513f, 0.8301887f, 1);
                case Team.Green:
                    return new Color(0.4079707f, 0.8018868f, 0.1929067f, 1);
                case Team.Yellow:
                    return new Color(0.8584906f, 0.5049533f, 0.2065237f, 1);
            }

            return new Color();
        }
    }
}
