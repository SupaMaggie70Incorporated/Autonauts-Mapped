using UnityEngine;
using UnityEngine.UI;

public class Effect2D : BaseClass
{
	protected Image m_Image;

	protected float m_Scale;

	protected float m_Timer;

	protected GameObject m_Target;

	protected Vector3 m_TargetOffset;

	protected Vector3 m_WorldPosition;

	public static bool GetIsTypeEffect2D(ObjectType NewType)
	{
		if (NewType == ObjectType.Emoticon || NewType == ObjectType.LoveHeart || NewType == ObjectType.MusicalNote || NewType == ObjectType.NewIcon || NewType == ObjectType.WallFloorIcon || NewType == ObjectType.XPPlus1 || NewType == ObjectType.StopActive)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Effect2D", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		m_Timer = 0f;
		m_Target = null;
		m_WorldPosition = default(Vector3);
	}

	protected new void Awake()
	{
		base.Awake();
		m_Scale = 1f;
		m_Image = GetComponent<Image>();
	}

	protected void SetSprite(string Name)
	{
		Sprite sprite = (Sprite)Resources.Load("Textures/Hud/" + Name, typeof(Sprite));
		m_Image.sprite = sprite;
	}

	public void SetTarget(GameObject Target, Vector3 TargetOffset)
	{
		m_Target = Target;
		m_TargetOffset = TargetOffset;
		UpdateTransform();
	}

	public void SetScale(float Scale)
	{
		m_Scale = Scale;
		UpdateTransform();
	}

	public void SetWorldPosition(Vector3 Position)
	{
		m_WorldPosition = Position;
		UpdateTransform();
	}

	public void UpdateTransform()
	{
		Vector3 vector = default(Vector3);
		vector = ((!m_Target) ? m_WorldPosition : (m_Target.transform.position + m_TargetOffset));
		float num = 1f / (CameraManager.Instance.m_Camera.transform.position - vector).magnitude;
		Vector3 position = CameraManager.Instance.m_Camera.WorldToScreenPoint(vector);
		if ((bool)HudManager.Instance)
		{
			base.transform.localPosition = HudManager.Instance.ScreenToCanvas(position);
		}
		float num2 = num * 20f * m_Scale;
		base.transform.localScale = new Vector3(num2, num2, num2);
	}

	private void Update()
	{
		UpdateTransform();
	}
}
