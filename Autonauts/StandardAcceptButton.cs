using UnityEngine;

public class StandardAcceptButton : StandardButtonImage
{
	protected new void Awake()
	{
		base.Awake();
		if ((bool)MyInputManager.Instance)
		{
			MyInputManager.Instance.AddAcceptButton(this);
		}
	}

	protected new void Start()
	{
		base.Start();
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
		if ((bool)MyInputManager.Instance)
		{
			MyInputManager.Instance.RemoveAcceptButton(this);
		}
	}

	public void KeyBindingPressed()
	{
		if (GetActive())
		{
			DoAction();
		}
	}

	public bool GetVisible()
	{
		Transform parent = base.transform;
		while ((bool)parent)
		{
			if (!parent.gameObject.activeSelf)
			{
				return false;
			}
			parent = parent.parent;
		}
		return true;
	}
}
