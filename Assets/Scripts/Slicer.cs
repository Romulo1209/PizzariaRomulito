using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpriteSlicer
{
    public class Slicer : MonoBehaviour
    {
        [SerializeField] private bool cutting = false;
        SpriteRenderer sr;
        TrailRenderer tr;

        public bool Cutting {get { return cutting; } set { cutting = value; } }

        Dictionary<Transform, (Vector2, Vector2)> records = new Dictionary<Transform, (Vector2, Vector2)>();

        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            tr = GetComponent<TrailRenderer>();

            Enable(false);
        }

        void Enable(bool _bool)
        {
            sr.enabled = _bool;
            tr.enabled = _bool;
        }

        void Update()
        {
            if(!cutting)
            {
                Enable(false);
                return;
            }
                

            Enable(true);
        }

        Vector2 GetMousePosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Cuttable"))
            {
                var startPos = transform.position;
                RecordTargetStart(collision.transform, startPos);
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Cuttable"))
            {
                if (collision.GetComponent<CuttableBase>().maxSlicesReached)
                {
                    return;
                }

                var endPos = transform.position;
                RecordTargetEnd(collision.transform, endPos);
                InformSliceManager(collision.transform);
            }
        }

        void RecordTargetStart(Transform _transform, Vector2 _start)
        {
            if (!records.ContainsKey(_transform))
            {
                records.Add(_transform, (_start, Vector2.zero));
            }
        }

        void RecordTargetEnd(Transform _transform, Vector2 _end)
        {
            if (records.ContainsKey(_transform))
            {
                var _start = records[_transform].Item1;
                records[_transform] = (_start, _end);
            }
        }

        void InformSliceManager(Transform _transform)
        {
            var pos = records[_transform];
            records.Remove(_transform);

            SlicesController.Instance.Slice(_transform, pos.Item1, pos.Item2);
        }

    }

}
