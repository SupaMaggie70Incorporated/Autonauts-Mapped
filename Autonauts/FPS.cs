using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
	private float deltaTime;

	private Text m_Text;

	private void Start()
	{
		m_Text = GetComponent<Text>();
	}

	private void Update()
	{
		deltaTime += (TimeManager.Instance.m_NormalDelta - deltaTime) * 0.1f;
		float num = deltaTime * 1000f;
		float num2 = 1f / deltaTime;
		string text = $"{num:0.0} ms ({num2:0.} fps)";
		m_Text.text = text;
	}
}
