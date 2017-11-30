using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Renderer))]
public class DustOff : Riddle, IBeginDragHandler, IDragHandler {
    [Space(10)]
    public Texture2D Brush;
    [Space(-10)]
    [Header("Alpha only ")]
    [Space(10)]

    private Texture2D _dustMask;

    private const int VisBrushSize = 256;
    private const int VisTextureSize = 1024;

    private const int CompBrushSize = VisBrushSize / 32;
    private const int CompTextureSize = VisTextureSize / 32;

    private Texture2D _visBrush;
    private RenderTexture _visTexture;

    private float[] _compBrush;
    private float[] _compTexture;

    private Collider _collider;
    private Material _material;

    private float _brightness;
    private float _threshold;
    private bool _fadeout;
    private float _reveal;

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            Ray ray = Camera.main.ScreenPointToRay(eventData.position);
            RaycastHit hit;

            if (_collider.Raycast(ray, out hit, Global.Constants.TouchDistance)) {
                //Adapted from: https://forum.unity3d.com/threads/painting-a-texture-ingame.323199/
                GL.PushMatrix();

                RenderTexture.active = _visTexture;
                GL.LoadPixelMatrix(0, VisTextureSize, VisTextureSize, 0);
                Vector2 coord = new Vector2(hit.textureCoord.x * VisTextureSize, VisTextureSize - hit.textureCoord.y * VisTextureSize);
                Graphics.DrawTexture(new Rect(coord.x - VisBrushSize / 2f, coord.y - VisBrushSize / 2f, VisBrushSize, VisBrushSize), _visBrush);

                RenderTexture.active = null;
                GL.PopMatrix();

                ApplyBrush(hit.textureCoord.x, hit.textureCoord.y);

                if (_brightness < _threshold) {
                    _fadeout = true;
                    Solved();
                }
            }
        }
    }

    private void Awake() {
        _collider = GetComponent<Collider>();

        _visBrush = Brush;
        _visTexture = new RenderTexture(VisTextureSize, VisTextureSize, 0);

        _material = GetComponent<MeshRenderer>().material;
        _dustMask = (Texture2D)_material.GetTexture("_TransTex");


        Graphics.Blit(_dustMask, _visTexture);

        Color[] compTexture = TextureScale.Bilinear(_dustMask, CompTextureSize, CompTextureSize).GetPixels();
        _compTexture = new float[compTexture.Length];
        for (int c = 0; c < compTexture.Length; c++) {
            _compTexture[c] = (compTexture[c].r + compTexture[c].g + compTexture[c].b) / 3 * compTexture[c].a;
        }

        Color[] compBrush = TextureScale.Bilinear(_visBrush, CompBrushSize, CompBrushSize).GetPixels();
        _compBrush = new float[compBrush.Length];
        for (int c = 0; c < compBrush.Length; c++) {
            _compBrush[c] = 1 - compBrush[c].a;
        }

        _brightness = GetTextureBrightness();
        _threshold = _brightness * 0.3f;
    }

    private void Update() {
        if (_fadeout) {
            _reveal += Time.deltaTime;
            _material.SetFloat("_Reveal", _reveal);

            if (_reveal > 1) {
                enabled = false;
            }
        }
    }

    private void ApplyBrush(float xCoord, float yCoord) {
        int xStart = (int)(xCoord * CompTextureSize) - CompBrushSize / 2;
        int yStart = (int)(yCoord * CompTextureSize) - CompBrushSize / 2;

        xStart = Mathf.Clamp(xStart, 0, CompTextureSize - CompBrushSize);
        yStart = Mathf.Clamp(yStart, 0, CompTextureSize - CompBrushSize);

        float brightnessDifference0 = 0;
        float brightnessDifference1 = 0;
        float brightnessDifference2 = 0;
        float brightnessDifference3 = 0;
        float brightnessDifference4 = 0;
        float brightnessDifference5 = 0;
        float brightnessDifference6 = 0;
        float brightnessDifference7 = 0;

        for (int bp = 0; bp < CompBrushSize; bp++) {
            int textureIndex = xStart + (yStart + bp) * CompTextureSize;
            int brushIndex = bp * CompBrushSize;

            //Loop unrolling            
            brightnessDifference0 += _compTexture[textureIndex];
            brightnessDifference1 += _compTexture[textureIndex + 1];
            brightnessDifference2 += _compTexture[textureIndex + 2];
            brightnessDifference3 += _compTexture[textureIndex + 3];
            brightnessDifference4 += _compTexture[textureIndex + 4];
            brightnessDifference5 += _compTexture[textureIndex + 5];
            brightnessDifference6 += _compTexture[textureIndex + 6];
            brightnessDifference7 += _compTexture[textureIndex + 7];

            _compTexture[textureIndex] *= _compBrush[brushIndex];
            _compTexture[textureIndex + 1] *= _compBrush[brushIndex + 1];
            _compTexture[textureIndex + 2] *= _compBrush[brushIndex + 2];
            _compTexture[textureIndex + 3] *= _compBrush[brushIndex + 3];
            _compTexture[textureIndex + 4] *= _compBrush[brushIndex + 4];
            _compTexture[textureIndex + 5] *= _compBrush[brushIndex + 5];
            _compTexture[textureIndex + 6] *= _compBrush[brushIndex + 6];
            _compTexture[textureIndex + 7] *= _compBrush[brushIndex + 7];

            brightnessDifference0 -= _compTexture[textureIndex];
            brightnessDifference1 -= _compTexture[textureIndex + 1];
            brightnessDifference2 -= _compTexture[textureIndex + 2];
            brightnessDifference3 -= _compTexture[textureIndex + 3];
            brightnessDifference4 -= _compTexture[textureIndex + 4];
            brightnessDifference5 -= _compTexture[textureIndex + 5];
            brightnessDifference6 -= _compTexture[textureIndex + 6];
            brightnessDifference7 -= _compTexture[textureIndex + 7];
        }

        _brightness -= brightnessDifference0 + brightnessDifference1 + brightnessDifference2 + brightnessDifference3 + brightnessDifference4 + brightnessDifference5 + brightnessDifference6 + brightnessDifference7;
    }

    private float GetTextureBrightness() {
        float brightness0 = 0;
        float brightness1 = 0;
        float brightness2 = 0;
        float brightness3 = 0;
        float brightness4 = 0;
        float brightness5 = 0;
        float brightness6 = 0;
        float brightness7 = 0;

        for (int p = 0; p < CompTextureSize * CompTextureSize; p += 8) {
            //Loop unrolling
            brightness0 += _compTexture[p];
            brightness1 += _compTexture[p + 1];
            brightness2 += _compTexture[p + 2];
            brightness3 += _compTexture[p + 3];
            brightness4 += _compTexture[p + 4];
            brightness5 += _compTexture[p + 5];
            brightness6 += _compTexture[p + 6];
            brightness7 += _compTexture[p + 7];
        }
        return brightness0 + brightness1 + brightness2 + brightness3 + brightness4 + brightness5 + brightness6 + brightness7;
    }
}