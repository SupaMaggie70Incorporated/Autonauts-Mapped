using UnityEngine;

public class StandardMainMenuButtonText : BaseButtonText
{
	public override void SetIndicated(bool Indicated)
	{
		base.SetIndicated(Indicated);
		float num = 1f;
		if (Indicated)
		{
			num = 1.2f;
		}
		base.transform.localScale = new Vector3(num, num, 1f);
	}
}
