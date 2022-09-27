using System.Collections.Generic;
using UnityEngine;

public class RefreshManager : MonoBehaviour
{
	public static RefreshManager Instance;

	private Dictionary<Actionable, int> m_Objects;

	private void Awake()
	{
		Instance = this;
		m_Objects = new Dictionary<Actionable, int>();
	}

	public void AddObject(Actionable NewObject)
	{
		if (!m_Objects.ContainsKey(NewObject))
		{
			m_Objects.Add(NewObject, 0);
		}
	}

	private void Update()
	{
		List<Actionable> list = new List<Actionable>();
		foreach (KeyValuePair<Actionable, int> @object in m_Objects)
		{
			list.Add(@object.Key);
		}
		m_Objects.Clear();
		foreach (Actionable item in list)
		{
			item.SendAction(new ActionInfo(ActionType.Refresh, default(TileCoord)));
		}
		LinkedSystemManager.Instance.UpdateAll();
	}
}
