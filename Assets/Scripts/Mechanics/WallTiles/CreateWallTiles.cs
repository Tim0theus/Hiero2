#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public enum WalltileRiddleType {
    No,
    Pickup,
    Slide
}

public class CreateWallTiles : MonoBehaviour {
    public WalltileRiddleType RiddleType;

    public Texture2D GlyphTexture;
    public Material BackgroundMaterial;

    public bool ScaleProportionally = true;

    [Range(0, 3)] public float XScale = 1;
    [Range(0, 3)] public float YScale = 1;

    [Range(-1, 1)] public float XOffset;
    [Range(-1, 1)] public float YOffset;

    public float Spacing = 0.1f;

    [Range(1, 5)] public int Columns = 4;
    [Range(1, 5)] public int Rows = 4;

    public int ScrableIterations = 16;

#if UNITY_EDITOR

    private void Editor_GenerateTileGrid() {
        Transform tilePrefab = Resources.Load<Transform>("Prefabs/Tile");
        Transform tileSlotBackgroundPrefab = Resources.Load<Transform>("Prefabs/TileSlotBackground");

        Editor_ClearTiles();

        Transform children = new GameObject("Children").transform;
        children.SetParent(transform, false);

        Transform tileBackground = new GameObject("TileBackground").transform;
        tileBackground.SetParent(transform, false);

        if (ScaleProportionally) {
            YScale = XScale;
        }

        ExtendedGlyph glyph = ResourceLoader.Get(GlyphTexture.name);

        for (int r = 0; r < Rows; r++) {
            for (int c = 0; c < Columns; c++) {
                Transform tileTransform = Instantiate(tilePrefab);
                tileTransform.name = string.Format("{0}-{1}x{2}-{3}_{4}", glyph.GlyphCode, Columns, Rows, c, r);
                tileTransform.SetParent(children, false);
                tileTransform.localPosition = new Vector3(c, r, 0) + new Vector3(c * Spacing, r * Spacing, 0);

                Tile tile = tileTransform.gameObject.AddComponent<Tile>();
                tile.OriginalCooridinates = new Vector2(c, r);


                #region Adjust Material

                Renderer renderer = tileTransform.GetComponent<Renderer>();
                Material material = new Material(renderer.sharedMaterial);

                float xScale = 1 / (XScale * Columns);
                float yScale = 1 / (YScale * Rows);

                float xOffset = 0.5f;
                float yOffset = 0.5f;

                if (ScaleProportionally) {
                    if (xScale < yScale) {
                        yScale = xScale;
                        yOffset -= (Rows - Columns) / (2f * Columns * XScale);
                    }
                    else {
                        xScale = yScale;
                        xOffset -= (Columns - Rows) / (2f * Rows * YScale);
                    }
                }

                xOffset += c * xScale;
                yOffset += r * yScale;

                xOffset -= 0.5f / XScale;
                yOffset -= 0.5f / YScale;

                xOffset -= XOffset / (2 * XScale);
                yOffset -= YOffset / (2 * YScale);

                material.mainTextureOffset = new Vector2(xOffset, yOffset);
                material.mainTextureScale = new Vector2(xScale, yScale);

                material.mainTexture = glyph.Hieroglyph.texture;
                renderer.material = material;

                #endregion


                Transform slotBackground = Instantiate(tileSlotBackgroundPrefab);
                slotBackground.SetParent(tileBackground, false);
                slotBackground.localPosition = new Vector3(c, r, 0) + new Vector3(c * Spacing, r * Spacing, 0.025f);
                slotBackground.name = tileSlotBackgroundPrefab.name;


                Renderer slotBackgroundRenderer = slotBackground.GetComponent<Renderer>();
                Material slotBackgroundMaterial = new Material(slotBackgroundRenderer.sharedMaterial);

                slotBackgroundMaterial.mainTexture = glyph.Transliteration.texture;
                slotBackgroundRenderer.material = slotBackgroundMaterial;
            }
        }

        GameObject background = GameObject.CreatePrimitive(PrimitiveType.Cube);
        DestroyImmediate(background.GetComponent<Collider>());
        background.name = "Background";
        background.GetComponent<Renderer>().material = BackgroundMaterial;
        background.transform.SetParent(transform, false);

        float positionX = ((Columns - 1) * (1 + Spacing)) / 2;
        float positionY = ((Rows - 1) * (1 + Spacing)) / 2;
        background.transform.localPosition = new Vector3(positionX, positionY, 0.08f);

        const float border = 0.2f;
        float scaleX = Columns + Columns * Spacing + border;
        float scaleY = Rows + Rows * Spacing + border;
        background.transform.localScale = new Vector3(scaleX, scaleY, 0.1f);

        AddRiddleAggregator();
    }

    private void AddRiddleAggregator() {
        switch (RiddleType) {
            case WalltileRiddleType.Pickup:
                PickupRiddleAggregator();
                break;

            case WalltileRiddleType.Slide:
                SlideTileRiddleAggregator();
                break;
        }
    }

    private void SlideTileRiddleAggregator() {
        SlideTileAggregator slideTileAggregator = gameObject.GetComponent<SlideTileAggregator>();
        if (slideTileAggregator == null) {
            slideTileAggregator = gameObject.AddComponent<SlideTileAggregator>();
        }
        else {
            slideTileAggregator.Riddles.Clear();
        }

        SlideTileMechanic slideTileMechanic = gameObject.AddComponent<SlideTileMechanic>();
        slideTileMechanic.Intialize(Rows, Columns);

        Transform slideTileChildren = transform.Find("Children");

        //Prepare every tile to move around
        foreach (Transform child in slideTileChildren) {
            Tile tile = child.GetComponent<Tile>();

            child.gameObject.AddComponent<SphereCollider>();
            child.SetParent(slideTileChildren);

            SlideTile slideTile = child.gameObject.AddComponent<SlideTile>();
            slideTile.OriginalCooridinates = tile.OriginalCooridinates;
            slideTile.CurrentCorrdinates = tile.OriginalCooridinates;
            slideTile.Mechanic = slideTileMechanic;

            HighlightTile highlightTile = child.GetComponent<HighlightTile>();

            slideTileAggregator.Riddles.Add(slideTile);
            slideTileAggregator.Indicators.Add(highlightTile);

            DestroyImmediate(tile);
        }

        //Convert a random tile into a Pickup
        int index = Random.Range(0, slideTileChildren.childCount);

        Transform freeTileTransform = slideTileChildren.GetChild(index);
        SlideTile freeTile = freeTileTransform.GetComponent<SlideTile>();
        DestroyImmediate(freeTileTransform.gameObject.GetComponent<Collider>());
        DestroyImmediate(freeTileTransform.gameObject.GetComponent<SlideTile>());

        CreateRigidBodyPickup createRigidBodySlidePickup = freeTileTransform.gameObject.AddComponent<CreateRigidBodyPickup>();
        GameObject pickUp = createRigidBodySlidePickup.Editor_ConvertPickup();
        pickUp.transform.position -= pickUp.transform.forward;

        PutDown putDown = slideTileChildren.GetComponentInChildren<PutDown>();
        SlideTile freeSlideTile = putDown.gameObject.AddComponent<SlideTile>();
        freeSlideTile.OriginalCooridinates = freeTile.OriginalCooridinates;
        freeSlideTile.CurrentCorrdinates = freeTile.OriginalCooridinates;
        freeSlideTile.Mechanic = freeTile.Mechanic;

        freeSlideTile.transform.localScale = Vector3.one;
        pickUp.transform.localScale = Vector3.one;
        freeTileTransform.transform.localScale = Vector3.one;

        SlideTile pickupSlideTile = pickUp.GetComponent<SlideTile>();
        Collider pickupCollider = pickUp.GetComponent<Collider>();
        slideTileAggregator.Riddles.Remove(pickupSlideTile);
        DestroyImmediate(freeTile);
        DestroyImmediate(createRigidBodySlidePickup);
        DestroyImmediate(pickupSlideTile);
        DestroyImmediate(pickupCollider);


        slideTileMechanic.FreeSlideTile = freeSlideTile;
        slideTileMechanic.Scramble(ScrableIterations);
    }

    private void PickupRiddleAggregator() {
        RiddleAggregator wallTileBase = GetComponent<RiddleAggregator>();
        if (wallTileBase == null) {
            wallTileBase = gameObject.AddComponent<RiddleAggregator>();
        }
        else {
            wallTileBase.Riddles.Clear();
        }

        Transform children = transform.Find("Children");

        foreach (Transform child in children) {
            CreateRigidBodyPickup createRigidBodyPickup = child.gameObject.AddComponent<CreateRigidBodyPickup>();
            createRigidBodyPickup.RiddleAggregator = wallTileBase;
            HighlightTile highlightTile = child.GetComponent<HighlightTile>();
            wallTileBase.Indicators.Add(highlightTile);
        }
    }

    private void Editor_ClearTiles() {
        while (transform.childCount > 0) {
            GameObject child = transform.GetChild(0).gameObject;
            DestroyImmediate(child);
        }

        if (GetComponent<RiddleAggregator>() != null) {
            DestroyImmediate(GetComponent<RiddleAggregator>());
        }
        if (GetComponent<SlideTileAggregator>() != null) {
            DestroyImmediate(GetComponent<SlideTile>());
        }
        if (GetComponent<SlideTileMechanic>() != null) {
            DestroyImmediate(GetComponent<SlideTileMechanic>());
        }
    }


    [CustomEditor(typeof(CreateWallTiles))]
    public class CreateWallTileEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            CreateWallTiles createWallTiles = (CreateWallTiles)target;

            if (GUILayout.Button("Generate")) {
                createWallTiles.Editor_GenerateTileGrid();
            }

            if (createWallTiles.ScaleProportionally) {
                createWallTiles.YScale = createWallTiles.XScale;
            }
        }
    }
#endif
}