using UnityEngine;

public class Instrument : Holdable
{
	protected string m_SoundName;

	protected Wobbler m_Wobbler;

	public static bool GetIsTypeInstrument(ObjectType NewType)
	{
		if (NewType == ObjectType.Triangle || NewType == ObjectType.Castanets || NewType == ObjectType.Cowbell || NewType == ObjectType.Guiro || NewType == ObjectType.Maracas || NewType == ObjectType.JawHarp || NewType == ObjectType.Guitar)
		{
			return true;
		}
		return false;
	}

	public override void RegisterClass()
	{
		base.RegisterClass();
		ClassManager.Instance.RegisterClass("Instrument", m_TypeIdentifier);
	}

	public override void Restart()
	{
		base.Restart();
		m_Wobbler.Restart();
		m_SoundName = VariableManager.Instance.GetVariableAsString(m_TypeIdentifier, "Sound");
	}

	protected new void Awake()
	{
		base.Awake();
		m_Wobbler = new Wobbler();
	}

	protected override void ActionDropped(Actionable PreviousHolder, TileCoord DropLocation)
	{
		base.ActionDropped(PreviousHolder, DropLocation);
		m_Wobbler.Restart();
	}

	public virtual void Play()
	{
		AudioManager.Instance.StartEvent(m_SoundName, this);
		m_Wobbler.Go(0.5f, 5f, 0.5f);
		Vector3 position = base.transform.position;
		ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.MusicalNote, position, Quaternion.identity).GetComponent<MusicalNote>().SetWorldPosition(position);
	}

	private void Update()
	{
		if (m_BeingHeld)
		{
			m_Wobbler.Update();
			base.transform.localPosition = new Vector3(0f, m_Wobbler.m_Height * 0.5f, 0f);
		}
	}
}
