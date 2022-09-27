using UnityEngine;

public class ToyMod : Toy
{
	public override void Restart()
	{
		base.Restart();
		Vector3 Scale = new Vector3(-1f, 1f, 1f);
		ModManager.Instance.ModToyClass.GetModelScale(m_TypeIdentifier, out Scale);
		m_ModelRoot.transform.localScale = new Vector3(Scale.x, Scale.y, Scale.z);
		if (ModManager.Instance.ModToyClass.GetModelRotation(m_TypeIdentifier, out var Rot))
		{
			m_ModelRoot.transform.localRotation = Quaternion.Euler(Rot.x, Rot.y, Rot.z);
		}
		if (ModManager.Instance.ModToyClass.GetModelTranslation(m_TypeIdentifier, out var Trans))
		{
			m_ModelRoot.transform.localPosition = new Vector3(Trans.x, Trans.y, Trans.z);
		}
	}
}
