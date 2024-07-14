using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace RTSToolkitFree
{
    public class UnitPars : MonoBehaviour
    {
        [FormerlySerializedAs("unitDesc")] public string desc;
        
        public bool isMovable = true;

        public bool isReady;
        public bool isApproaching;
        public bool isAttacking;
        [HideInInspector] public bool isApproachable = true;
        public bool isImmune;
        public bool isDying;
        public bool isSinking;

        public UnitPars target;
        public List<UnitPars> attackers = new();

        public int maxAttackers = 3;

        [HideInInspector] public float prevR;
        [HideInInspector] public int failedR;
        public int critFailedR = 100;

        public float health = 100.0f;
        public float maxHealth = 100.0f;
        public float selfHealFactor = 10.0f;

        public float strength = 10.0f;
        public float defence = 10.0f;

        [HideInInspector] public int deathCalls;
        public int maxDeathCalls = 5;

        [HideInInspector] public bool changeMaterial = true;

        public int nation = 1;

        [SerializeField] private TMP_Text display;

        void Start()
        {
            NavMeshAgent nma = GetComponent<NavMeshAgent>();
            
            if (nma != null)
            {
                nma.enabled = true;
            }
        }

        public void Init(Unit u)
        {
            name = u.name;
            desc = u.description;
            health = maxHealth = u.maxHealth;
            defence = u.defence;
            strength = u.strength;
            selfHealFactor = u.selfHealFactor;

            display.text = u.visualData.fallbackAsciiArt;
        }
    }
}
