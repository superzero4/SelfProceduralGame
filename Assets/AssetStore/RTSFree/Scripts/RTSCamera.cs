﻿using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RTSToolkitFree
{
    public class RTSCamera : MonoBehaviour
    {
        [HideInInspector] public Terrain terrain;
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float rotationSpeed = 2f;
        [SerializeField] private float scrollSpeed = 5f;

        [SerializeField] private float minAbsoluteHeight;
        [SerializeField] private float minHeightAboveTerrain = 2f;
        [SerializeField] private float maxHeightAboveTerrain = 200f;
        
        [SerializeField] private SpawnPoint buildingPrefab;
        [SerializeField] private LayerMask terrainMask;
        
        [NonSerialized] public Building SelectedBuilding;
        private Team currentTeam = Team.Red;
        private Camera cam;

        public List<Button> buttons = new();
        private List<string> spawnedBuildings = new();
        private bool canPlaceBuidlings = true;
        
        void Awake()
        {
            terrain = FindObjectOfType<Terrain>();
            cam = GetComponent<Camera>();
        }

        void Update()
        {
            Move();
            Rotate();
            Zoom();
            Click();

            if (Input.GetKeyDown(KeyCode.X)) canPlaceBuidlings = !canPlaceBuidlings;
        }

        private void Move()
        {
            Vector3 movingDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                movingDirection += transform.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                movingDirection += Vector3.Cross(transform.forward, Vector3.up).normalized * transform.forward.magnitude;
            }
            if (Input.GetKey(KeyCode.S))
            {
                movingDirection += -transform.forward;
            }
            if (Input.GetKey(KeyCode.D))
            {
                movingDirection += -Vector3.Cross(transform.forward, Vector3.up).normalized * transform.forward.magnitude;
            }

            float h = terrain.SampleHeight(transform.position);
            if (h < 0f)
            {
                h = 0f;
            }

            float y = transform.position.y - h;
            Vector3 velocity = movingDirection * (Time.deltaTime * y * moveSpeed);
            transform.position += velocity;
            float newHeight = terrain.SampleHeight(transform.position);

            if (newHeight < 0f)
            {
                newHeight = 0f;
            }

            transform.position = new Vector3(transform.position.x, newHeight + y, transform.position.z);
        }

        private void Rotate()
        {
            if (Input.GetMouseButton(1))
            {
                float h = -rotationSpeed * Input.GetAxis("Mouse X");
                float v = rotationSpeed * Input.GetAxis("Mouse Y");

                transform.Rotate(0, h, 0, Space.World);
                transform.Rotate(v, 0, 0);

                if ((transform.rotation.eulerAngles.z >= 160) && (transform.rotation.eulerAngles.z <= 200))
                {
                    transform.Rotate(-v, 0, 0);
                }
            }
        }

        private void Zoom()
        {
            float msw = Input.GetAxis("Mouse ScrollWheel");
            if (msw != 0)
            {
                float h = terrain.SampleHeight(transform.position);
                if (h < minAbsoluteHeight)
                {
                    h = minAbsoluteHeight;
                }
                float y = transform.position.y - h;

                transform.position += transform.forward * (msw * scrollSpeed * y * Time.deltaTime);

                if (y < minHeightAboveTerrain)
                {
                    transform.position = new Vector3(transform.position.x, h + minHeightAboveTerrain, transform.position.z);
                }
                else if (y > maxHeightAboveTerrain)
                {
                    transform.position = new Vector3(transform.position.x, h + maxHeightAboveTerrain, transform.position.z);
                }
            }
        }

        private void Click()
        {
            if (canPlaceBuidlings && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 500, terrainMask))
                {
                    var newBuidling = Instantiate(buildingPrefab, hitInfo.point, Quaternion.identity);
                    newBuidling.transform.up = hitInfo.normal;

                    if (DataInit.UnitLookup.ContainsKey(SelectedBuilding.name))
                    {
                        newBuidling.Init(SelectedBuilding, DataInit.UnitLookup[SelectedBuilding.name], currentTeam);
                    }
                    else
                    {
                        newBuidling.Init(SelectedBuilding, null, currentTeam);
                    }

                    if (!spawnedBuildings.Contains(SelectedBuilding.name))
                    {
                        spawnedBuildings.Add(SelectedBuilding.name);
                    }

                    UpdateButtons();
                }
            }
        }

        public void UpdateButtons()
        {
            var data = DataInit.Data.buildings;
            for (int i = 0; i < data.Length; i++)
            {
                buttons[i].interactable = !data[i].dependencies.Except(spawnedBuildings).Any();
            }
        }
        
        public void SpawnBuilding(int i)
        {
            SelectedBuilding = DataInit.Data.buildings[i];
        }

        public void SetTeam(int i)
        {
            currentTeam = (Team)i;
        }
    }
}

public enum Team {Red, Blue, Green, Yellow}