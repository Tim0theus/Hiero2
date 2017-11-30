using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlideTileMechanic : MonoBehaviour {

    public int Columns;
    public int Rows;

    public SlideTile FreeSlideTile;
    public List<AudioClip> SlideSounds;

    private AudioSource _audioSource;
    private int _lockCount;
    private bool[] _moveable;

    private void Start() {
        _audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        _moveable = new bool[Rows * Columns];

        UpdateFreeFlags(true);
    }

    public void UpdateRiddle(SlideTile tile) {
        int x = (int)tile.CurrentCorrdinates.x;
        int y = (int)tile.CurrentCorrdinates.y;

        int index = y * Columns + x;

        if (_moveable[index] && _lockCount == 0) {
            UpdateFreeFlags(false);

            tile.SwapWith(FreeSlideTile);
            FreeSlideTile.SwapWith(tile);

            Vector2 tempCoordinates = tile.CurrentCorrdinates;
            tile.CurrentCorrdinates = FreeSlideTile.CurrentCorrdinates;
            FreeSlideTile.CurrentCorrdinates = tempCoordinates;

            UpdateFreeFlags(true);

            if (SlideSounds.Count > 0) {
                index = Random.Range(0, SlideSounds.Count);
                _audioSource.PlayOneShot(SlideSounds[index]);
            }
        }
    }

    private void UpdateFreeFlags(bool value) {
        int x = (int)FreeSlideTile.CurrentCorrdinates.x;
        int y = (int)FreeSlideTile.CurrentCorrdinates.y;

        int rightIndex = x + 1;
        int leftIndex = x - 1;
        int aboveIndex = y + 1;
        int belowIndex = y - 1;

        if (0 <= rightIndex && rightIndex < Columns) {
            _moveable[y * Columns + rightIndex] = value;
        }
        if (0 <= leftIndex && leftIndex < Columns) {
            _moveable[y * Columns + leftIndex] = value;
        }
        if (0 <= aboveIndex && aboveIndex < Rows) {
            _moveable[aboveIndex * Columns + x] = value;
        }
        if (0 <= belowIndex && belowIndex < Rows) {
            _moveable[belowIndex * Columns + x] = value;
        }
    }

    public void Lock() {
        _lockCount++;
    }

    public void UnLock() {
        _lockCount--;
    }

    public void Intialize(int rows, int columns) {
        Rows = rows;
        Columns = columns;
    }

    public void Scramble(int iterations) {
        int[] indices = new int[Rows * Columns];
        for (int i = 0; i < indices.Length; i++) {
            indices[i] = i;
        }

        List<int> candidates = new List<int>();

        int current = LinearIndex(FreeSlideTile.OriginalCooridinates);
        int previous = current;

        for (int i = 0; i < iterations; i++) {
            int rightIndex = current + 1;
            int leftIndex = current - 1;
            int aboveIndex = current + Columns;
            int belowIndex = current - Columns;

            if (rightIndex < Rows * Columns && rightIndex % Columns != 0) {
                candidates.Add(rightIndex);
            }
            if (0 <= leftIndex && current % Columns != 0) {
                candidates.Add(leftIndex);
            }
            if (aboveIndex < Rows * Columns) {
                candidates.Add(aboveIndex);
            }
            if (0 <= belowIndex) {
                candidates.Add(belowIndex);
            }

            candidates.Remove(previous);
            previous = current;
            current = candidates[Random.Range(0, candidates.Count)];

            int temp = indices[previous];
            indices[previous] = indices[current];
            indices[current] = temp;


            candidates.Clear();
        }

        SlideTile[] slideTiles = gameObject.GetComponentsInChildren<SlideTile>();
        Dictionary<int, SlideTile> tiles = slideTiles.ToDictionary(item => LinearIndex(item.OriginalCooridinates));
        Dictionary<int, Vector3> positions = slideTiles.ToDictionary(item => LinearIndex(item.OriginalCooridinates), item => item.transform.position);
        Dictionary<int, Vector2> coordinates = slideTiles.ToDictionary(item => LinearIndex(item.OriginalCooridinates), item => item.OriginalCooridinates);


        for (int i = 0; i < indices.Length; i++) {
            SlideTile slideTile = tiles[indices[i]];

            slideTile.transform.position = positions[i];
            slideTile.CurrentCorrdinates = coordinates[i];
        }
    }

    private int LinearIndex(Vector2 coordinates) {
        return (int)(coordinates.x + coordinates.y * Columns);
    }
}