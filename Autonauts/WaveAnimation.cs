using UnityEngine;

public class WaveAnimation : MonoBehaviour
{
	public void StartAnimation()
	{
		ShorelineManager.Instance.StartWaves();
	}

	private void Update()
	{
	}
}
