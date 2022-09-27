using UnityEngine;

public class AutopediaPage : MonoBehaviour
{
	public void SetActive(bool Active)
	{
		base.gameObject.SetActive(Active);
	}

	public bool GetActive()
	{
		return base.gameObject.activeSelf;
	}

	public virtual void CeremonyPlaying(bool Playing, Quest NewQuest)
	{
	}
}
