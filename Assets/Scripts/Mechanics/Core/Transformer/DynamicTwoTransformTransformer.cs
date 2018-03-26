using System;
using UnityEngine;

public class DynamicTwoTransformTransformer : MonoBehaviour {
    public bool IsInTransition {
        get { return enabled; }
    }

    private event EventHandler Finished;

    private Transform _from;
    private Transform _to;

    private Vector3 _fromScale;
    private Vector3 _toScale;

    private ITransformable _transformable;
    private float _transition;
    private float _speed;

    public static DynamicTwoTransformTransformer Create(Transform item, float speed = 1) {
        DynamicTwoTransformTransformer transformer = item.gameObject.AddComponent<DynamicTwoTransformTransformer>();
        transformer.enabled = false;
        transformer._transformable = new ProportionalScaleTransform(item);
        transformer._speed = speed;

        return transformer;
    }

    private void Update() {
        if(_transition <= 1) {
            _transition += Time.deltaTime * _speed;

            _transformable.Position = Vector3.Lerp(_from.position, _to.position, _transition);
            _transformable.Rotation = Quaternion.Lerp(_from.rotation, _to.rotation, _transition);
            _transformable.LocalScale = Vector3.Lerp(_fromScale, _toScale, _transition);
        }
        else {
            _transformable.Position = _to.position;
            _transformable.Rotation = _to.rotation;
            _transformable.LocalScale = _toScale;

            _transformable.Parent = _to;

            enabled = false;

            if (Finished != null) {
                Finished(this, null);
                Finished = null;
            }

        }
    }

    public void Set(Transform from, Transform to, EventHandler eventHandler = null) {
        _from = from;
        _to = to;
        Finished += eventHandler;

        Vector3 startScale = _from.lossyScale;
        Vector3 endScale = _to.lossyScale;

        _fromScale = Vector3.one;
        _toScale = new Vector3(Math.Abs(endScale.x / startScale.x), Math.Abs(endScale.y / startScale.y), Math.Abs(endScale.z / startScale.z));
    }

    public void Activate() {
        _transition = 0;
        enabled = true;
    }
}