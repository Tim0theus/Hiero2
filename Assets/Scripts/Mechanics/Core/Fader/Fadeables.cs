using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IFadeable {
    GameObject GameObject { get; }
    Color Color { set; }
    void Show();
    void Hide();
    void Disable();
}

public class ImageFadeable : IFadeable {
    public GameObject GameObject { get; private set; }
    public Color Color { get { return _image.color; } set { _image.color = value; } }

    private readonly Image _image;
    public ImageFadeable(Image image) {
        GameObject = image.gameObject;
        _image = image;
    }

    public void Show() {
        _image.enabled = true;
    }

    public void Hide() {
        _image.enabled = false;
    }

    public void Disable() { }
}

public class MaterialFadeable : IFadeable {
    public GameObject GameObject { get; private set; }
    public Color Color {
        get { return _materials[0].color; }
        set {
            foreach (Material material in _materials) {
                material.color = value;
            }
        }
    }

    private readonly List<Material> _materials = new List<Material>();
    private readonly Renderer _renderer;

    public MaterialFadeable(Renderer renderer) {
        GameObject = renderer.gameObject;
        _renderer = renderer;

        foreach (Material material in renderer.materials) {
            _materials.Add(material);
        }
    }

    public void Show() {
        _renderer.enabled = true;
    }

    public void Hide() {
        _renderer.enabled = false;
    }

    public void Disable() { }
}

public class EmissiveMaterialFadeable : IFadeable {
    public GameObject GameObject { get; private set; }
    public Color Color {
        get {
            Color currentRGB = _materials[0].GetColor("_EmissionColor");
            float currentA = _materials[0].color.a;
            return new Color(currentRGB.r, currentRGB.g, currentRGB.b, currentA);
        }
        set {
            foreach (Material material in _materials) {
                material.SetColor("_EmissionColor", value);
                material.EnableKeyword("_EMISSION");
                Color color = material.color;
                color.a = value.a;
                material.color = color;
            }
        }
    }

    private readonly List<Material> _materials = new List<Material>();
    private readonly Renderer _renderer;

    public EmissiveMaterialFadeable(Renderer renderer) {
        GameObject = renderer.gameObject;
        _renderer = renderer;

        foreach (Material material in renderer.materials) {
            _materials.Add(material);
        }
    }

    public void Show() {
        _renderer.enabled = true;
    }

    public void Hide() {
        _renderer.enabled = false;
    }

    public void Disable() {
        foreach (Material material in _materials) {
            material.DisableKeyword("_EMISSION");
        }
    }
}
public class TextFadeable : IFadeable {
    public GameObject GameObject { get; private set; }
    public Color Color { get { return _text.color; } set { _text.color = value; } }

    private readonly Text _text;
    public TextFadeable(Text text) {
        GameObject = text.gameObject;
        _text = text;
    }

    public void Show() {
        _text.enabled = true;
    }

    public void Hide() {
        _text.enabled = false;
    }

    public void Disable() { }
}
