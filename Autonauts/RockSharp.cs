using UnityEngine;

public class RockSharp : Holdable
{
	public override void PostCreate()
	{
		base.PostCreate();
		Vector3 localScale = base.transform.localScale;
		localScale = localScale * 0.75f + localScale * Random.Range(0f, 0.5f);
		base.transform.localScale = localScale;
	}
}
