using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothness = 4f;
    [SerializeField] private float maxDeflection = 30;
    [SerializeField] private bool smoothRotationOn = true;

    private Vector3 offset;
    private Quaternion initialRotation;
    private Coroutine fixDeflection;
    private float initialSmoothness;

    private void Start()
    {
        offset = target.position - transform.position;
        initialRotation = transform.rotation;
        initialSmoothness = smoothness;
    }

    private void Update()
    {
        if (target == null) return;
        var position = target.position - (target.rotation * offset);
        var rotation = target.rotation * initialRotation;

        transform.position = position;

        if (smoothRotationOn)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothness * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, rotation) > maxDeflection && fixDeflection == null)
            {
                fixDeflection = StartCoroutine(FixDeflection());
            }
        }
        else
        {
            transform.rotation = rotation;
        }
    }

    private IEnumerator FixDeflection()
    {
        while (Quaternion.Angle(transform.rotation, target.rotation * initialRotation) > maxDeflection)
        {
            smoothness++;
            yield return new WaitForSeconds(0.5f);
        }

        smoothness = initialSmoothness;
        fixDeflection = null;
    }
}