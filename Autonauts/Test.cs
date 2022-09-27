using UnityEngine;

public class Test : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		base.transform.rotation *= Quaternion.Euler(0f, 360f * Time.deltaTime, 0f);
	}
}
