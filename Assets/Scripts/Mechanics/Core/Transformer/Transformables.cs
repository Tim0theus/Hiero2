
using System;
using UnityEngine;

public interface ITransformable {
    Vector3 Position { get; set; }
    Quaternion Rotation { get; set; }
    Vector3 LocalScale { get; set; }

    Transform Parent { get; set; }
}


public class DirectTransform : ITransformable {
    private readonly Transform _transform;

    public Vector3 Position {
        get { return _transform.position; }
        set { _transform.position = value; }
    }

    public Quaternion Rotation {
        get { return _transform.rotation; }
        set { _transform.rotation = value; }
    }

    public Vector3 LocalScale {
        get { return _transform.localScale; }
        set { _transform.localScale = value; }
    }

    public Transform Parent {
        get { return _transform.parent; }
        set { _transform.SetParent(value); }
    }

    public DirectTransform(Transform transform) {
        _transform = transform;
    }
}

public class ProportionalScaleTransform : ITransformable {
    private readonly Transform _transform;

    public Vector3 Position {
        get { return _transform.position; }
        set { _transform.position = value; }
    }

    public Quaternion Rotation {
        get { return _transform.rotation; }
        set { _transform.rotation = value; }
    }

    public Vector3 LocalScale {
        get { return _transform.localScale; }
        set {
            float absX = Math.Abs(value.x);
            float absY = Math.Abs(value.y);
            float absZ = Math.Abs(value.z);

            float largestTransformScale = Math.Max(Math.Max(_transform.localScale.x, _transform.localScale.z), _transform.localScale.y);
            float smallestValueScale = Math.Min(Math.Min(absX, absY), absZ);

            float factor = smallestValueScale / largestTransformScale;

            _transform.localScale = _transform.localScale * factor;
        }
    }

    public Transform Parent {
        get { return _transform.parent; }
        set { _transform.SetParent(value); }
    }

    public ProportionalScaleTransform(Transform transform) {
        _transform = transform;
    }
}