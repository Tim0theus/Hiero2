using System;
using System.Collections.Generic;
using UnityEngine;

public class Transformer : MonoBehaviour {
    private ITransformable _transformable;

    private readonly List<TransformerPoint> _points = new List<TransformerPoint>();

    private TransformerPoint _from;
    private TransformerPoint _to;

    private int _cursor;

    private int TransitionCursor {
        get { return _cursor; }
        set {
            if (value + 1 < _points.Count) {
                _cursor = value;
                _from = _points[_cursor];
                _to = _points[_cursor + 1];
            }
        }
    }


    private int _direction = 1;
    private float _transition;

    private int _bounceProgressCounter;

    private void Update() {
        if (0 <= _transition && _transition <= 1) {
            _transition += Time.deltaTime * _direction * _to.Speed;

            _transformable.Position = Vector3.Lerp(_from.Transform.position, _to.Transform.position, _transition);
            _transformable.Rotation = Quaternion.Lerp(_from.Transform.rotation, _to.Transform.rotation, _transition);
            _transformable.LocalScale = Vector3.Lerp(_from.Scale, _to.Scale, _transition);
        }
        else {
            _transition = Mathf.Clamp01(_transition);

            if (_bounceProgressCounter < _to.BounceCount) {
                _direction *= -1;
                _bounceProgressCounter++;
            }
            else {
                enabled = false;
                _bounceProgressCounter = 0;

                _to.OnTransitionFinished();
                _transformable.Parent = _to.Transform;

                TransitionCursor++;
            }
        }
    }

    public void AddPoint(Transform to, float speed = 1, int bounceCount = 0) {
        AddPoint(to, null, speed, bounceCount);
    }

    public void AddPoint(Transform to, EventHandler eventHandler, float speed = 1, int bounceCount = 0) {
        Vector3 scale = to.localScale;

        if (_points.Count > 0) {
            Vector3 startScale = _points[_points.Count - 1].Transform.lossyScale;
            Vector3 endScale = to.lossyScale;

            scale = new Vector3(Math.Abs(endScale.x / startScale.x), Math.Abs(endScale.y / startScale.y), Math.Abs(endScale.z / startScale.z));
        }

        _points.Add(new TransformerPoint(to, scale, speed, bounceCount, eventHandler));

        if (TransitionCursor + 3 == _points.Count) {
            TransitionCursor++;
        }
    }

    public void Activate() {
        _direction = 1;
        _transition = 0;
        _bounceProgressCounter = 0;
        TransitionCursor = 0;
        enabled = true;
    }

    public void Reactivate() {
        _transition = 0;
        _bounceProgressCounter = 0;
        enabled = true;
    }

    public static Transformer Create(Transform item, Transform from, Transform to) {
        Transformer transformer = Create(item, from);

        transformer.AddPoint(to);

        return transformer;
    }

    public static Transformer Create(Transform item, Transform from) {
        Transformer transformer = Create(item);

        transformer.AddPoint(from);

        return transformer;
    }

    public static Transformer Create(Transform item) {
        Transformer transformer = item.gameObject.AddComponent<Transformer>();
        transformer.enabled = false;
        transformer._transformable = new ProportionalScaleTransform(item);

        return transformer;
    }
}

public class TransformerPoint {
    public Flag Flag { get; private set; }
    public Transform Transform { get; private set; }
    public Vector3 Scale { get; private set; }
    public float Speed { get; private set; }
    public int BounceCount { get; private set; }
    private event EventHandler TransitionFinished;

    public TransformerPoint(Transform transform, Vector3 scale, float speed = 1, int bounceCount = 0, EventHandler transitionFinished = null) {
        Transform = transform;
        Scale = scale;
        Speed = speed;
        BounceCount = Math.Max(0, bounceCount * 2 - 1);
        TransitionFinished = transitionFinished;
    }

    public TransformerPoint(Transform transform, Flag flag, Vector3 scale, float speed = 1, int bounceCount = 0, EventHandler transitionFinshed = null) : this(transform, scale, speed, bounceCount, transitionFinshed) {
        Flag = flag;
    }

    public void OnTransitionFinished() {
        if (TransitionFinished != null) TransitionFinished(this, EventArgs.Empty);
    }
}

