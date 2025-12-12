using System.Collections;
using System.Collections.Generic;
using SpriteSlicer;
using UnityEngine;

public class SlicesController : MonoBehaviour
{
    [SerializeField] private GameObject pizzaPrefab;
    [SerializeField] private Transform pizzaSpawnPointTransform;
    [SerializeField] private List<PizzaSlice> pizzaSlices;
    [SerializeField] private Material sliceMaterial;

    public int PizzaSlicesCount { get { return pizzaSlices.Count;}}
    public List<PizzaSlice> PizzaSlices { get { return pizzaSlices; } }
    public Material SliceMaterial { get { return sliceMaterial; } }

    public static SlicesController Instance { get { return instance; } }

    static SlicesController instance;
    [Range(0, 10)] public float force = 0;

    void Awake()
    {
        instance = this;
    }

    public void Slice(Transform _target, Vector2 _startPos, Vector2 _endPos)
    {
        var sliceEngine = new SliceEngine(_target, _startPos, _endPos);

        sliceEngine.Slice();
    }

    public void RegisterPizzaSlice(GameObject slice)
    {
        var pizzaSlice = slice.GetComponent<PizzaSlice>();
        if (pizzaSlice != null)
            pizzaSlices.Add(pizzaSlice);
    }

    public void GeneratePizza()
    {
        if(GameController.Instance.OnTutorialMode)
            return;

        StartCoroutine(GeneratePizzaCoroutine());
        return;
    }

    IEnumerator GeneratePizzaCoroutine()
    {
        foreach(var slice in pizzaSlices)
        {
            Vector2 dir = Vector2.up + new Vector2(Random.Range(-1f, 1f), 0f);
            dir.Normalize();
            slice.GetComponent<Rigidbody2D>().AddForce(dir * 50, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(2f);

        foreach(var slice in pizzaSlices)
            Destroy(slice.gameObject);

        var pizza = Instantiate(pizzaPrefab, pizzaSpawnPointTransform.position, Quaternion.identity);
        pizzaSlices.Clear();
        RegisterPizzaSlice(pizza);
    }
}
