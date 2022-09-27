using UnityEngine;

public class MedicineMod : Medicine
{
	public override void Restart()
	{
		base.Restart();
		Vector3 Scale = new Vector3(-1f, 1f, 1f);
		ModManager.Instance.ModMedicineClass.GetModelScale(m_TypeIdentifier, out Scale);
		m_ModelRoot.transform.localScale = new Vector3(Scale.x, Scale.y, Scale.z);
		if (ModManager.Instance.ModMedicineClass.GetModelRotation(m_TypeIdentifier, out var Rot))
		{
			m_ModelRoot.transform.localRotation = Quaternion.Euler(Rot.x, Rot.y, Rot.z);
		}
		if (ModManager.Instance.ModMedicineClass.GetModelTranslation(m_TypeIdentifier, out var Trans))
		{
			m_ModelRoot.transform.localPosition = new Vector3(Trans.x, Trans.y, Trans.z);
		}
	}
}
