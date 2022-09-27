using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class SpawnAnimationManager : MonoBehaviour
{
	public static SpawnAnimationManager Instance;

	private Dictionary<BaseClass, SpawnAnimation> m_Objects;

	private List<Worker> m_AbortList;

	private void Awake()
	{
		Instance = this;
		m_Objects = new Dictionary<BaseClass, SpawnAnimation>();
	}

	public void ReadySave()
	{
		List<SpawnAnimation> list = new List<SpawnAnimation>();
		foreach (KeyValuePair<BaseClass, SpawnAnimation> @object in m_Objects)
		{
			SpawnAnimation value = @object.Value;
			if (!value.IsSavable())
			{
				list.Add(value);
			}
		}
		m_AbortList = new List<Worker>();
		foreach (SpawnAnimation item in list)
		{
			if (item.m_Type == SpawnAnimation.Type.Jump)
			{
				Actionable finishTarget = ((SpawnAnimationJump)item).m_FinishTarget;
				if ((bool)finishTarget && finishTarget.m_TypeIdentifier == ObjectType.Worker)
				{
					m_AbortList.Add(finishTarget.GetComponent<Worker>());
				}
			}
			item.Abort();
			m_Objects.Remove(item.m_NewObject);
		}
	}

	public void SaveFinished()
	{
		foreach (Worker abort in m_AbortList)
		{
			abort.m_WorkerInterpreter.RestartLastInstruction();
		}
	}

	public void Save(JSONNode Node)
	{
		JSONArray jSONArray = (JSONArray)(Node["Objects"] = new JSONArray());
		int num = 0;
		foreach (KeyValuePair<BaseClass, SpawnAnimation> @object in m_Objects)
		{
			SpawnAnimation value = @object.Value;
			JSONNode jSONNode2 = new JSONObject();
			value.Save(jSONNode2);
			jSONArray[num] = jSONNode2;
			num++;
		}
	}

	public void Load(JSONNode Node)
	{
		m_Objects.Clear();
		JSONArray asArray = Node["Objects"].AsArray;
		for (int i = 0; i < asArray.Count; i++)
		{
			JSONNode asObject = asArray[i].AsObject;
			SpawnAnimation.Type asInt = (SpawnAnimation.Type)JSONUtils.GetAsInt(asObject, "Type", 0);
			SpawnAnimation spawnAnimation = null;
			switch (asInt)
			{
			case SpawnAnimation.Type.Jump:
				spawnAnimation = new SpawnAnimationJump();
				break;
			case SpawnAnimation.Type.ShoreIn:
				spawnAnimation = new SpawnAnimationShoreIn();
				break;
			case SpawnAnimation.Type.ShoreOut:
				spawnAnimation = new SpawnAnimationShoreOut();
				break;
			}
			spawnAnimation.Load(asObject);
			if (spawnAnimation.m_NewObject != null)
			{
				Add(spawnAnimation);
			}
		}
	}

	public void PostLoad()
	{
		foreach (KeyValuePair<BaseClass, SpawnAnimation> @object in m_Objects)
		{
			@object.Value.PostLoad();
		}
	}

	public void Add(SpawnAnimation NewAnimation)
	{
		NewAnimation.Start();
		m_Objects.Add(NewAnimation.m_NewObject, NewAnimation);
		NewAnimation.Update();
	}

	public void AddJump(BaseClass NewObject, TileCoord StartPosition, TileCoord EndPosition, float StartHeight, float EndHeight, float JumpHeight, float Delay = 0.2f, Actionable FinishTarget = null, bool DustLand = false, Actionable Spawner = null, Actionable Target = null)
	{
		SpawnAnimation newAnimation = new SpawnAnimationJump(NewObject, StartPosition, EndPosition, StartHeight, EndHeight, JumpHeight, 0f, Delay, FinishTarget, DustLand, Spawner, Target);
		Add(newAnimation);
	}

	public void AddJump(BaseClass NewObject, Vector3 StartPosition, Vector3 EndPosition, float JumpHeight, float Delay = 0.2f, Actionable FinishTarget = null, bool DustPuff = false, Actionable Spawner = null, Actionable Target = null)
	{
		SpawnAnimation newAnimation = new SpawnAnimationJump(NewObject, StartPosition, EndPosition, JumpHeight, 0f, Delay, FinishTarget, DustPuff, Spawner, Target);
		Add(newAnimation);
	}

	public void AddShoreIn(BaseClass NewObject, TileCoord TilePosition, Vector3 MovementDirection)
	{
		SpawnAnimation newAnimation = new SpawnAnimationShoreIn(NewObject, TilePosition, MovementDirection);
		Add(newAnimation);
	}

	public void AddShoreOut(BaseClass NewObject, TileCoord TilePosition, Vector3 MovementDirection)
	{
		SpawnAnimation newAnimation = new SpawnAnimationShoreOut(NewObject, TilePosition, MovementDirection);
		Add(newAnimation);
	}

	private void Remove(SpawnAnimation NewAnimation, bool Success)
	{
		NewAnimation.End(Success);
		m_Objects.Remove(NewAnimation.m_NewObject);
	}

	public bool GetIsObjectSpawning(BaseClass TestObject)
	{
		return m_Objects.ContainsKey(TestObject);
	}

	public SpawnAnimation GetAnimation(BaseClass TestObject)
	{
		if (!m_Objects.ContainsKey(TestObject))
		{
			return null;
		}
		return m_Objects[TestObject];
	}

	public void StopObjectSpawning(BaseClass NewObject)
	{
		if (m_Objects.ContainsKey(NewObject))
		{
			m_Objects.Remove(NewObject);
		}
	}

	public Vector3 GetObjectTargetLocation(BaseClass TestObject)
	{
		Vector3 result = default(Vector3);
		if (m_Objects.ContainsKey(TestObject))
		{
			SpawnAnimation spawnAnimation = m_Objects[TestObject];
			if (spawnAnimation.m_NewObject == TestObject && spawnAnimation.m_Type == SpawnAnimation.Type.Jump)
			{
				return ((SpawnAnimationJump)spawnAnimation).m_EndPosition;
			}
		}
		Debug.Log("SpawnAnimationManager : Couldn't find object");
		return result;
	}

	public bool GetObjectAtTile(TileCoord NewCoord)
	{
		foreach (KeyValuePair<BaseClass, SpawnAnimation> @object in m_Objects)
		{
			SpawnAnimation value = @object.Value;
			if ((bool)value.m_NewObject && (bool)value.m_NewObject.GetComponent<Selectable>() && value.m_NewObject.GetComponent<Selectable>().m_TileCoord == NewCoord)
			{
				return true;
			}
		}
		return false;
	}

	public void Remove(BaseClass NewObject)
	{
		if (m_Objects.ContainsKey(NewObject))
		{
			m_Objects.Remove(NewObject);
		}
	}

	private void Update()
	{
		List<SpawnAnimation> list = new List<SpawnAnimation>();
		foreach (KeyValuePair<BaseClass, SpawnAnimation> @object in m_Objects)
		{
			SpawnAnimation value = @object.Value;
			if (!value.Update())
			{
				list.Add(value);
			}
		}
		foreach (SpawnAnimation item in list)
		{
			Remove(item, Success: true);
		}
	}
}
