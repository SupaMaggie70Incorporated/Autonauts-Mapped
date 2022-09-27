using UnityEngine;

public class ConverterRequirementFuel : MonoBehaviour
{
	private BaseText m_CountText;

	private BaseText m_FuelText;

	private BaseProgressBar m_Fuel;

	private BaseImage m_Image;

	private void Awake()
	{
	}

	private void CheckGadgets()
	{
		if (m_Image == null)
		{
			m_Image = base.transform.Find("Image").GetComponent<BaseImage>();
			m_Fuel = base.transform.Find("ProgressBar").GetComponent<BaseProgressBar>();
			m_CountText = base.transform.Find("Count").GetComponent<BaseText>();
			m_FuelText = base.transform.Find("Fuel").GetComponent<BaseText>();
		}
	}

	public void SetTier(BurnableInfo.Tier NewTier, bool WhiteText = true)
	{
		CheckGadgets();
		ObjectType objectTypeFromTier = BurnableInfo.GetObjectTypeFromTier(NewTier);
		m_Image.SetSprite(IconManager.Instance.GetIcon(objectTypeFromTier));
		Color colour = new Color(1f, 1f, 1f);
		if (!WhiteText)
		{
			colour = new Color(0f, 0f, 0f);
		}
		m_CountText.SetColour(colour);
		m_FuelText.SetColour(colour);
	}

	public void UpdateFuel(float Percent, int Value)
	{
		CheckGadgets();
		m_Fuel.SetValue(Percent);
		m_CountText.SetText(Value.ToString());
	}
}
