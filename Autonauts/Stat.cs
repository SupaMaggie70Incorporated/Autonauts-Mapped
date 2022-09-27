using UnityEngine;
using UnityEngine.UI;

public class Stat : MonoBehaviour
{
	public void SetStat(StatsManager.Stat NewStat)
	{
		base.transform.Find("Title").GetComponent<Text>().text = StatsManager.Instance.GetStatTitle(NewStat);
		base.transform.Find("Amount").GetComponent<Text>().text = StatsManager.Instance.GetStat(NewStat);
	}
}
