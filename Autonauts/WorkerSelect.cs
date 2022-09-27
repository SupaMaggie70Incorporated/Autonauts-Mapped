using UnityEngine;

public class WorkerSelect : MonoBehaviour
{
	private void Start()
	{
		RectTransform component = GetComponent<RectTransform>();
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/BackButton", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, base.transform).transform.localPosition = new Vector3(component.rect.width / 2f - 30f, component.rect.height / 2f - 30f, 0f);
	}

	public void OnClickPickup()
	{
		GameStateSelectWorker component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateSelectWorker>();
		CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().GoToAndAction(component.m_TilePosition, component.m_TargetObject);
		GameStateManager.Instance.PopState();
	}

	public void OnClickTeach()
	{
		GameStateManager.Instance.PopState();
	}
}
