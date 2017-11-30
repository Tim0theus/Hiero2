using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Region {
    public Vector2 Center { get; protected set; }
    public abstract bool IsInside(Vector2 position);
    public abstract void Draw();
    public virtual void SetCenter(Vector2 position) {
        Center = position;
    }
}

public class CicleRegion : Region {
    private readonly float _radius;
    private readonly int _axisDeadZone;

    private readonly List<Vector2> _points = new List<Vector2>();

    public CicleRegion(Vector2 center, float radius, int axisDeadZone = 0) {
        Center = center;
        _radius = Math.Abs(radius);
        _axisDeadZone = Math.Abs(axisDeadZone);

        SetPoints();
    }

    public override bool IsInside(Vector2 position) {
        Vector2 positionFromCenter = position - Center;

        bool insideCircle = positionFromCenter.sqrMagnitude < _radius * _radius;

        bool outsideDeadzoneX = Math.Abs(positionFromCenter.x) > _axisDeadZone;
        bool outsideDeadzoneY = Math.Abs(positionFromCenter.y) > _axisDeadZone;

        return insideCircle && outsideDeadzoneY && outsideDeadzoneX;
    }
    public override void SetCenter(Vector2 position) {
        Center = position;

        SetPoints();
    }
    public override void Draw() {
        for (int i = 0; i < _points.Count - 1; i++) {
            Debug.DrawLine(_points[i], _points[i + 1], Color.green);
        }
        Debug.DrawLine(_points[_points.Count - 1], _points[0], Color.green);

    }
    private void SetPoints() {
        const int segments = 32;
        const float angle = Mathf.Deg2Rad * (360f / segments);

        Vector2 point = Vector2.zero;

        for (int i = 0; i <= segments; i++) {
            float x = Mathf.Sin(angle * i) * _radius;
            float y = Mathf.Cos(angle * i) * _radius;

            point.x = Center.x + x;
            point.y = Center.y + y;

            _points.Add(point);
        }
    }
}

public class RectangularRegion : Region {
    public Vector2 UpperRight {
        get { return _upperRight; }
    }

    public Vector2 LowerLeft {
        get { return _lowerLeft; }
    }

    private readonly float _widthFromCenter;
    private readonly float _heightFromCenter;
    private readonly int _axisDeadZone;

    private Vector2 _upperLeft;
    private Vector2 _upperRight;
    private Vector2 _lowerLeft;
    private Vector2 _lowerRight;


    public RectangularRegion(Vector2 center, float widthFromCenter, float heightFromCenter, int axisDeadZone = 0) {
        Center = center;
        _widthFromCenter = Math.Abs(widthFromCenter);
        _heightFromCenter = Math.Abs(heightFromCenter);
        _axisDeadZone = Math.Abs(axisDeadZone);

        SetBounds();
    }

    public RectangularRegion(Vector2 lowerLeft, Vector2 upperRight, int axisDeadZone = 0) {
        Center = (upperRight - lowerLeft) / 2 + lowerLeft;
        _widthFromCenter = Math.Abs((upperRight.x - lowerLeft.x) / 2);
        _heightFromCenter = Math.Abs((upperRight.y - lowerLeft.y) / 2);
        _axisDeadZone = Math.Abs(axisDeadZone);

        SetBounds();
    }


    public override bool IsInside(Vector2 position) {
        float absPositionFromCenterX = Math.Abs((position - Center).x);
        float absPositionFromCenterY = Math.Abs((position - Center).y);

        bool insideRectX = absPositionFromCenterX < _widthFromCenter;
        bool insideRectY = absPositionFromCenterY < _heightFromCenter;

        bool outsideDeadzoneX = absPositionFromCenterX > _axisDeadZone;
        bool outsideDeadzoneY = absPositionFromCenterY > _axisDeadZone;

        return insideRectX && insideRectY && outsideDeadzoneY && outsideDeadzoneX;
    }
    public override void SetCenter(Vector2 position) {
        Center = position;
        SetBounds();
    }
    public override void Draw() {
        Draw(Color.green);
    }
    public void Draw(Color color) {
        Debug.DrawLine(_upperLeft, _upperRight, color);
        Debug.DrawLine(_upperRight, _lowerRight, color);
        Debug.DrawLine(_lowerRight, _lowerLeft, color);
        Debug.DrawLine(_lowerLeft, _upperLeft, color);
    }
    private void SetBounds() {
        _upperLeft = Center + new Vector2(-_widthFromCenter, _heightFromCenter);
        _upperRight = Center + new Vector2(_widthFromCenter, _heightFromCenter);
        _lowerLeft = Center + new Vector2(-_widthFromCenter, -_heightFromCenter);
        _lowerRight = Center + new Vector2(_widthFromCenter, -_heightFromCenter);
    }
}
