using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private TMP_Text _name, _description, _replacementText, _infoText;
    [SerializeField] private Image _image;
    [SerializeField] private Image _border;
    
    public void SetVisuals(TileController.VisualInformation visualInformation, bool forceActive=true)
    {
        if (forceActive)
            gameObject.SetActive(true);
        if (visualInformation.visuals.sprite != null)
        {
            if (_image.sprite != null)
                _image.sprite = visualInformation.visuals.sprite;
            _replacementText.text = "";
        }
        else
        {
            _replacementText.text = visualInformation.visuals.fallbackAsciiArt;
            _image.sprite = null;
        } //Works also as a safe, even if text is empty

        _name.text = visualInformation.name;
        _description.text = visualInformation.description;
        _border.color = visualInformation.name.ToConstantRGB();
        //Linke break every two values
        _infoText.text = string.Join("\n", visualInformation.values.Select((f,i) => f.ToString("F3") /*+(i%2==0 ? "\n":"")*/));
    }
}