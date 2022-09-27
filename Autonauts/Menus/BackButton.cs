using UnityEngine.UI;

public class BackButton : Button
{
	protected new void Awake()
	{
		base.onClick.AddListener(delegate
		{
			OnClick();
		});
	}

	private void OnClick()
	{
		AudioManager.Instance.StartEvent("UIOptionCancelled");
		GameStateManager.Instance.PopState();
	}

	public void OnMouseEnter()
	{
		AudioManager.Instance.StartEvent("UIOptionIndicated");
	}
}
