using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttableBase : MonoBehaviour
{
    public bool maxSlicesReached = false;

        [Header("Cuttable Shader Parameters")]
        public List<(float, float)> parameters = new List<(float, float)>();
        [Range(2, 8)] [SerializeField] int maxSliceAllowed = 8;

        [Header("Slice Info")]
        [SerializeField] public float slicePercentage;

        SpriteRenderer sr;

        public virtual void Start()
        {
            NullCheck();
        }

        void NullCheck()
        {
            if (sr == null)
            {
                sr = GetComponent<SpriteRenderer>();
                sr.material = SlicesController.Instance.SliceMaterial;
            }
        }

        public void SetNewShaderParameters(float _degree, float _edge)
        {
            parameters.Add((_degree, _edge));
            maxSlicesReached = (parameters.Count >= maxSliceAllowed) ? true : false;

            UpdateShaderParameters();
        }

        void UpdateShaderParameters()
        {
            NullCheck();

            var sliceIndex = 1;
            foreach (var param in parameters)
            {
                sr.material.SetFloat("_Degree_" + sliceIndex, param.Item1);
                sr.material.SetFloat("_Edge_" + sliceIndex, param.Item2);

                sliceIndex++;
            }

        }

        public void InvokeEnableCollider()
        {
            Invoke("EnableCollider", 0.15f);
        }

        void EnableCollider()
        {
            GetComponent<PolygonCollider2D>().enabled = true;
        }

        public void ExertForce(Vector2 dir)
        {
            GetComponent<Rigidbody2D>().AddForce(dir * SlicesController.Instance.force);
        }
}
