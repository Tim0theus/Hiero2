using System;
using UnityEngine;

public class DynamicTwoPositionTransformer : MonoBehaviour {
    private event EventHandler Finished;

    private Vector3 _from;
    private Vector3 _to;

    private ITransformable _transformable;
    private float _transition;
    private float _speed;

    public static DynamicTwoPositionTransformer Create(Transform item, float speed = 1) {
        if (!Application.isPlaying) return null;
        DynamicTwoPositionTransformer transformer = item.gameObject.GetComponent<DynamicTwoPositionTransformer>() ?? item.gameObject.AddComponent<DynamicTwoPositionTransformer>();
        transformer.enabled = false;
        transformer._transformable = new DirectTransform(item);
        transformer._speed = speed;

        return transformer;
    }

    private void Update() {
        if (_transition <= 1) {
            _transition += Time.deltaTime * _speed;

            _transformable.Position = Vector3.Lerp(_from, _to, _transition);
        }
        else {
            _transformable.Position = _to;

            if (Finished != null) {
                Finished(null, null);
            }

            enabled = false;
        }
    }

    public void Set(Vector3 from, Vector3 to, EventHandler eventHandler = null) {
        _from = from;
        _to = to;
        Finished = eventHandler;
    }

    public void Activate() {
        _transition = 0;
        enabled = true;
    }
}