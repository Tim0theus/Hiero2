using UnityEngine;

public class Inventory : MonoBehaviour {
    public static Transform Slot { get; private set; }
    public static Item Item { get; private set; }

    public static bool HasFreeSlot {
        get { return !_lock && _currentRiddleCode == string.Empty; }
    }

    private static string _currentRiddleCode = string.Empty;

    private static bool _lock;

    public static bool Contains(string riddleCode) {
        return _currentRiddleCode == riddleCode;
    }

    public static void Add(string riddleCode, Item item) {
        _currentRiddleCode = riddleCode;
        Item = item;
    }

    public static void Lock() {
        _lock = true;
    }

    public static void UnLock() {
        _lock = false;
    }

    public static void Clear() {
        _currentRiddleCode = string.Empty;
        Item = null;
    }

    private void Awake() {
        Slot = transform.GetChild(0);
    }

    public static void Drop(Vector3 worldPosition, Vector3 worldNormal) {
        if (Item != null) {
            Item.Drop(worldPosition, worldNormal);
            Clear();
        }
    }
}