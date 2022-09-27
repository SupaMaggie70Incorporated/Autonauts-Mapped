using UnityEngine;

public class Start : MonoBehaviour
{
	private void Awake()
	{
		BaseButton component = base.transform.Find("StartButton").GetComponent<BaseButton>();
		component.SetAction(OnButtonClicked, component);
		AudioManager.Instance.StartMusic("MusicCover");
	}

	public void OnButtonClicked(BaseGadget NewGadget)
	{
		GameStateManager.Instance.SetState(GameStateManager.State.MainMenu);
	}
}
