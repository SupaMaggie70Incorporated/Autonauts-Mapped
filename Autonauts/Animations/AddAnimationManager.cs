using System.Collections.Generic;
using UnityEngine;

public class AddAnimationManager : MonoBehaviour
{
	public static AddAnimationManager Instance;

	private Dictionary<BaseClass, AddAnimation> m_Animations;

	private void Awake()
	{
		Instance = this;
		m_Animations = new Dictionary<BaseClass, AddAnimation>();
	}

	public void Add(BaseClass NewObject, bool Adding)
	{
		if (ObjectTypeList.Instance.GetIsBuilding(NewObject.m_TypeIdentifier))
		{
			NewObject = NewObject.GetComponent<Building>().GetTopBuilding();
		}
		if (!m_Animations.ContainsKey(NewObject))
		{
			m_Animations.Add(NewObject, new AddAnimation(NewObject, Adding));
		}
	}

	public void Remove(BaseClass NewObject)
	{
		if (m_Animations.ContainsKey(NewObject))
		{
			m_Animations.Remove(NewObject);
		}
	}

	public bool IsAnimating(BaseClass NewObject)
	{
		return m_Animations.ContainsKey(NewObject);
	}

	private void Update()
	{
		List<BaseClass> list = new List<BaseClass>();
		foreach (KeyValuePair<BaseClass, AddAnimation> animation in m_Animations)
		{
			AddAnimation value = animation.Value;
			BaseClass key = animation.Key;
			if (!value.Update())
			{
				list.Add(key);
			}
		}
		foreach (BaseClass item in list)
		{
			m_Animations.Remove(item);
		}
	}
}
