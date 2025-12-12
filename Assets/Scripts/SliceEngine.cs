using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpriteSlicer
{
    public class SliceEngine
    {
        Transform target;
        Transform second;
        Vector2 startPos;
        Vector2 endPos;

        float alpha = 5.0f;
        bool suspend = false;

        Vector2 velocity;
        float angularVelocity;

        Rigidbody2D r1;
        Rigidbody2D r2;

        public SliceEngine(Transform _target, Vector2 _startPos, Vector2 _endPos)
        {
            target = _target;
            startPos = _startPos;
            endPos = _endPos;
        }

        public void Slice()
        {
            StopMovement();
            CalcAccuratePoints();
            UpdateCollider();

            if (suspend) return;
            UpdateShader();
            UpdateRigidBody();
        }

        void StopMovement()
        {
            r1 = target.GetComponent<Rigidbody2D>();

            velocity = r1.velocity;
            r1.velocity = Vector2.zero;

            angularVelocity = r1.angularVelocity;
            r1.angularVelocity = 0;
        }


        void CalcAccuratePoints()
        {
            var dir = endPos - startPos;
            var extentedStartPoint = startPos - (alpha * dir);
            var extentedEndPoint = endPos + (alpha * dir);

            var newPoint = GetHitPoint(extentedStartPoint, -dir);
            startPos = (newPoint != Vector2.zero) ? newPoint : startPos;

            newPoint = GetHitPoint(extentedEndPoint, dir);
            endPos = (newPoint != Vector2.zero) ? newPoint : endPos;
        }

        Vector2 GetHitPoint(Vector2 start, Vector2 dir)
        {
            var hits = Physics2D.RaycastAll(start, -dir);
            foreach (var hit in hits)
            {
                if (hit.transform == target)
                {
                    return hit.point;
                }
            }

            return Vector2.zero;
        }

        void UpdateShader()
        {
            target.GetComponent<PolygonCollider2D>().enabled = false;

            var _degree = CalcAngle() + 90f - target.eulerAngles.z;
            var _edge = CalcDistance(startPos, (endPos - startPos), target.position);

            var _params = target.GetComponent<CuttableBase>().parameters;
            if (_params != null && _params.Count > 0)
            {
                second.GetComponent<CuttableBase>().parameters.AddRange(_params);
            }

            target.GetComponent<CuttableBase>().SetNewShaderParameters(_degree, _edge);
            second.GetComponent<CuttableBase>().SetNewShaderParameters(_degree + 180, _edge * -1);
        }

        void UpdateCollider()
        {
            target.GetComponent<PolygonCollider2D>().enabled = false;

            var colliderPoints = target.GetComponent<PolygonCollider2D>().points;

            var _points1 = new List<Vector2>();
            var _points2 = new List<Vector2>();
            var dir = 0;

            var rotFactor = Quaternion.AngleAxis(target.eulerAngles.z, Vector3.back);

            var _startPos = startPos - (Vector2)target.position;
            var _endPos = endPos - (Vector2)target.position;

            _startPos = rotFactor * _startPos;
            _endPos = rotFactor * _endPos;

            bool splitPointsAdded1 = false;
            bool splitPointsAdded2 = false;

            foreach (var point in colliderPoints)
            {
                if (CalcDistance(_startPos, (_endPos - _startPos), point) > 0)
                {
                    if (dir == -1)
                    {
                        _points1.Add(_endPos);
                        _points1.Add(_startPos);
                        splitPointsAdded1 = true;
                    }
                    _points1.Add(point);
                    dir = 1;
                }
                else
                {
                    if (dir == 1)
                    {
                        _points2.Add(_startPos);
                        _points2.Add(_endPos);
                        splitPointsAdded2 = true;
                    }
                    _points2.Add(point);
                    dir = -1;
                }
            }

            if(!splitPointsAdded1)
            {
                _points1.Add(_endPos);
                _points1.Add(_startPos);
            }
            if (!splitPointsAdded2)
            {
                _points2.Add(_startPos);
                _points2.Add(_endPos);
            }

            if (_points1.Count > 2 && _points2.Count > 2)
            {
                target.GetComponent<PolygonCollider2D>().points = _points1.ToArray();

                second = GameObject.Instantiate(target, target.position, target.rotation);
                second.name = target.name + "_" + Random.Range(100, 999);
                second.GetComponent<PolygonCollider2D>().points = _points2.ToArray();
                second.GetComponent<CuttableBase>().InvokeEnableCollider();
                SlicesController.Instance.RegisterPizzaSlice(second.gameObject);
            }
            else
            {
                suspend = true;
                target.GetComponent<Rigidbody2D>().velocity = velocity;
            }

            target.GetComponent<CuttableBase>().InvokeEnableCollider();
        }

        void UpdateRigidBody()
        {
            r2 = second.GetComponent<Rigidbody2D>();

            r1.velocity = velocity;
            r2.velocity = velocity;

            r1.angularVelocity = angularVelocity;
            r2.angularVelocity = angularVelocity;

            var forceDir = Vector2.Perpendicular((endPos - startPos).normalized);

            target.GetComponent<CuttableBase>().ExertForce(forceDir * -1f);
            second.GetComponent<CuttableBase>().ExertForce(forceDir);
        }


        float CalcAngle()
        {
            var lineVec = endPos - startPos;
            return Vector2.SignedAngle(Vector2.right, lineVec);
        }

        float CalcDistance(Vector2 linePoint, Vector2 lineVec, Vector2 point)
        {
            Vector2 lineToCentre = (linePoint - point);
            return Vector2.Dot(lineToCentre, Vector2.Perpendicular(lineVec.normalized));
        }
    }
}

