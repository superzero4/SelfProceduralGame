using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine.Serialization;

namespace Game
{
    [Serializable]
    public struct FinalData
    {
        public Building[] buildings;
        public Unit[] units;

        public static FinalData FromJson(string json)
        {
            FinalData tmp = JsonUtility.FromJson<FinalData>(json);
            for (var i = 0; i < tmp.buildings.Length; i++)
            {
                var building = tmp.buildings[i];
                building.dependenciesReferences = tmp.BuildDependenciesReferences(building.dependencies);
                //In case json interpreted a sprite someway ?
                building.visualData.sprite = null;
                tmp.buildings[i] = building;
            }
            for (var i = 0; i < tmp.units.Length; i++)
            {
                var unit = tmp.units[i];
                unit.dependenciesReferences = tmp.BuildDependenciesReferences(unit.dependencies);
                //In case json interpreted a sprite someway ?
                unit.visualData.sprite = null;
                tmp.units[i] = unit;
            }
            return tmp;
        }

        private Building[] BuildDependenciesReferences(string[] names)
        {
            var tmp = new List<Building>(names.Length);
            foreach (var build in buildings)
                if (names.Contains(build.name))
                    tmp.Add(new Building(build));
            return tmp.ToArray();
        }

        public static FinalData Sample()
        {
            return new FinalData()
            {
                buildings = new [] { Building.Sample(), Building.Sample() },
                units = new [] { Unit.Sample(), Unit.Sample(), Unit.Sample() }
            };
        }
    }

    [Serializable]
    public struct VisualData
    {
        [FormerlySerializedAs("texture")] public Sprite sprite;
        public string fallbackAsciiArt;
    }

    [Serializable]
    public struct Unit
    {
        public string name;
        public string description;
        [NonSerialized] public Building[] dependenciesReferences;

        public string[] dependencies;
        public VisualData visualData;

        public static Unit Sample()
        {
            return new Unit { dependencies = Building.SampleDependency() };
        }
    }

    [Serializable]
    public struct Building
    {
        public string name;
        public string description;
        [NonSerialized] //For json
        [ReadOnly] //For editor view
        public Building[] dependenciesReferences;
        public string[] dependencies;
        public VisualData visualData;

        public Building(Building toCopy)
        {
            name = toCopy.name;
            description = toCopy.description;
            dependencies = toCopy.dependencies;
            visualData = toCopy.visualData;
            dependenciesReferences = null;
            //We don't copy references because it's struct we don't have guarantee they won't change, we just keep name in case we need to rebuild them
        }

        public static Building Sample()
        {
            return new Building { dependencies = SampleDependency() };
        }

        public static string[] SampleDependency()
        {
            //Watch out for stack overflow calling a new defaultly initialized Building and not another sample as dependency
            return new [] { "nameOfDependency1", "nameOfDependency2" };
        }
    }
}