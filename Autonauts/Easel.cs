using UnityEngine;

public class Easel : Converter
{
	private PlaySound m_PlaySound;

	private MyParticles m_Particles;

	public override void Restart()
	{
		base.Restart();
		m_DisplayIngredients = true;
		SetDimensions(new TileCoord(0, 0), new TileCoord(0, 0), new TileCoord(0, 1));
		SetSpawnPoint(new TileCoord(1, 0));
	}

	protected override Vector3 GetIngredientPosition(BaseClass NewObject)
	{
		if (NewObject.m_TypeIdentifier == ObjectType.Canvas)
		{
			return m_IngredientsRoot.transform.position;
		}
		return default(Vector3);
	}

	protected override void EndAddAnything(AFO Info)
	{
		bool flag = false;
		int num = 0;
		if ((bool)Info.m_Object && (Info.m_Object.m_TypeIdentifier == ObjectType.PaintBlue || Info.m_Object.m_TypeIdentifier == ObjectType.PaintRed || Info.m_Object.m_TypeIdentifier == ObjectType.PaintYellow))
		{
			flag = true;
			num = Info.m_Object.GetComponent<Holdable>().m_UsageCount;
		}
		base.EndAddAnything(Info);
		if (flag)
		{
			num++;
			if (num != VariableManager.Instance.GetVariableAsInt(ObjectType.ToolBucketMetal, "MaxUsage"))
			{
				TileCoord spawnPoint = GetSpawnPoint();
				Vector3 position = spawnPoint.ToWorldPositionTileCentered();
				BaseClass baseClass = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.ToolBucketMetal, position, Quaternion.identity);
				baseClass.GetComponent<Holdable>().m_UsageCount = num;
				TileCoord spawnPointEject = GetSpawnPointEject();
				SpawnAnimationManager.Instance.AddJump(baseClass, spawnPointEject, spawnPoint, 0f, baseClass.transform.position.y, 4f, 0.2f, null, DustLand: false, this);
			}
		}
	}

	protected override void UpdateIngredients()
	{
		m_DisplayIngredients = false;
		base.UpdateIngredients();
		m_DisplayIngredients = true;
		foreach (Holdable ingredient in m_Ingredients)
		{
			if (ingredient.m_TypeIdentifier == ObjectType.Canvas)
			{
				ingredient.SendAction(new ActionInfo(ActionType.Show, default(TileCoord), this));
				ingredient.transform.SetParent(m_IngredientsRoot);
				ingredient.transform.localPosition = default(Vector3);
				ingredient.transform.localRotation = Quaternion.identity;
				break;
			}
		}
	}

	public override void StartConverting()
	{
		base.StartConverting();
		m_PlaySound = AudioManager.Instance.StartEvent("BuildingWorkbenchMaking", this, Remember: true);
		m_Particles = ParticlesManager.Instance.CreateParticles("EaselPaint", m_IngredientsRoot.transform.position, base.transform.rotation * Quaternion.Euler(-90f, 0f, 0f));
		ParticlesManager.Instance.DestroyParticles(m_Particles, WaitUntilNoParticles: true);
	}

	protected override void UpdateConverting()
	{
		if ((int)(m_StateTimer * 60f) % 20 < 10)
		{
			m_ModelRoot.transform.localScale = new Vector3(0.9f, 1.3f, 0.9f);
		}
		else
		{
			m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		UpdateConvertAnimTimer(0.33f);
	}

	protected override void EndConverting()
	{
		AudioManager.Instance.StopEvent(m_PlaySound);
		AudioManager.Instance.StartEvent("BuildingToolMakingComplete", this);
		m_ModelRoot.transform.localScale = new Vector3(1f, 1f, 1f);
		m_Particles.Stop();
	}

	public override void DoConvertAnimAction()
	{
		base.DoConvertAnimAction();
		UpdateIngredients();
	}
}
