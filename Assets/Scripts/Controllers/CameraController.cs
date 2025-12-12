using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform mainPositionTransform;
    [SerializeField] private Transform outsidePositionTransform;

    private Coroutine moveCoroutine;

    public static CameraController Instance { get { return instance; } }
    static CameraController instance;

    void Awake()
    {
        instance = this;
    }

    public void MoveToOutsidePosition()
    {
        MoveToPositionSmooth(outsidePositionTransform);
    }

    public void MoveToMainPosition()
    {
        MoveToPositionSmooth(mainPositionTransform);
    }

    private void MoveToPositionSmooth(Transform targetPosition)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveToPositionCoroutine(targetPosition));
    }

    private IEnumerator MoveToPositionCoroutine(Transform targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 0.5f;
        Vector3 startingPos = transform.position;
        Vector3 targetPos = targetPosition.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPos, targetPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }
}
