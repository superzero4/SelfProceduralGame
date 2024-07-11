using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RTSToolkitFree
{
    public class UnitPars : MonoBehaviour
    {
        public string unitDesc;
        
        public bool isMovable = true;

        public bool isReady;
        public bool isApproaching;
        public bool isAttacking;
        [HideInInspector] public bool isApproachable = true;
        public bool isHealing;
        public bool isImmune;
        public bool isDying;
        public bool isSinking;

        public UnitPars target;
        public List<UnitPars> attackers = new List<UnitPars>();

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

        void Start()
        {
            NavMeshAgent nma = GetComponent<NavMeshAgent>();
            
            if (nma != null)
            {
                nma.enabled = true;
            }
        }
    }
}
