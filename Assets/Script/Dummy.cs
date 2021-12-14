using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public enum DummyType { Auto, Rotating, Sliding }
    public DummyType type;
    public int orderNumber;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BallCollided();
        }
    }

    private void BallCollided()
    {
        StartCoroutine(SizeShift());
        if (type == DummyType.Auto)
            StartCoroutine(RotateShift());
    }

    public void MoveDummy(float changeVolume)
    {
        changeVolume /= 1000;
        if (type == DummyType.Rotating)
        {
            transform.Rotate(Vector3.up, changeVolume);
        }
        else if (type == DummyType.Sliding)
        {
            transform.localPosition += new Vector3(0, 0, changeVolume/100);
            if(transform.localPosition.z > 0.375f)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, .375f);
            }
            if (transform.localPosition.z < -0.375f)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -.375f);
            }
        }
        else
            Debug.LogError("Auto Dummy should not have been targeted.");
    }

    private IEnumerator SizeShift()
    {
        float scaleZ = transform.localScale.z;
        float newScaleZ = scaleZ;
        for (int i = 0; i < 10; i++)
        {
            newScaleZ *= 0.95f;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x, transform.localScale.y, newScaleZ), 0.5f);
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 10; i++)
        {
            newScaleZ *= 1.05f;
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(transform.localScale.x, transform.localScale.y, newScaleZ), 0.5f);
            yield return new WaitForSeconds(0.01f);
        }
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, scaleZ);
    }
    private IEnumerator RotateShift()
    {
        float rotateX = transform.localRotation.eulerAngles.x;
        float newRotateX = rotateX;
        for (int i = 0; i < 10; i++)
        {
            newRotateX += 1.5f;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(newRotateX, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z), 0.5f);
            yield return new WaitForSeconds(0.005f);
        }
        for (int i = 0; i < 20; i++)
        {
            newRotateX -= 1.5f;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(newRotateX, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z), 0.5f);
            yield return new WaitForSeconds(0.005f);
        }
        for (int i = 0; i < 10; i++)
        {
            newRotateX += 1.5f;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(newRotateX, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z), 0.5f);
            yield return new WaitForSeconds(0.005f);
        }
        transform.localRotation = Quaternion.Euler(rotateX, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
    }
}
