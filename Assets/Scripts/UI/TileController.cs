using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TileController : MonoBehaviour
{
    [SerializeField, AssetsOnly] private Tile _tilePrefab;

    [FormerlySerializedAs("transform")] [SerializeField]
    private LayoutGroup root;

    private List<Tile> _tiles = new();

    [Serializable]
    public struct VisualInformation
    {
        public string name, description;
        [FormerlySerializedAs("_visuals")] public VisualData visuals;
        public float[] values;
    }
    public void UpdateTiles(IEnumerable<VisualInformation> select)
    {
        if (_tiles.Count == 0)
        {
            foreach (var v in select)
            {
                var t = NewTile();
                t.SetVisuals(v,true);
            }
        }else
            UpdateTiles(select.ToList());
    }

    private void UpdateTiles(IList<VisualInformation> select)
    {
        int diff = select.Count - _tiles.Count;
        if (diff > 0)
            for (int i = 0; i < diff; i++)
                NewTile();
        else if (diff < 0)
            for (int i = select.Count; i < _tiles.Count; i++)
                _tiles[i].gameObject.SetActive(false);
        for (int i = 0; i < select.Count; i++)
            _tiles[i].SetVisuals(select[i], true);
    }

    private Tile NewTile()
    {
        var t = GameObject.Instantiate(_tilePrefab, root.transform);
        _tiles.Add(t);
        return t;
    }
}