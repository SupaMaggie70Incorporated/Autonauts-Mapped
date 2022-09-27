using System.Collections.Generic;
using UnityEngine;

public class WorkerSelectOptions : MonoBehaviour
{
	private Farmer m_Farmer;

	private void Awake()
	{
		RectTransform component = GetComponent<RectTransform>();
		Object.Instantiate((GameObject)Resources.Load("Prefabs/Hud/BackButton", typeof(GameObject)), new Vector3(0f, 0f, 0f), Quaternion.identity, base.transform).transform.localPosition = new Vector3(component.rect.width / 2f - 30f, component.rect.height / 2f - 30f, 0f);
	}

	private void OnDestroy()
	{
	}

	public void SetInfo(Farmer NewFarmer)
	{
		m_Farmer = NewFarmer;
	}

	public void TradeButtonClicked()
	{
		GameStateManager.Instance.SetState(GameStateManager.State.Inventory);
		List<BaseClass> players = CollectionManager.Instance.GetPlayers();
		GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>().SetInfo(players[0].GetComponent<FarmerPlayer>(), m_Farmer);
	}

	public void TeachButtonClicked()
	{
		GameStateManager.Instance.SetState(GameStateManager.State.TeachWorker);
	}
}
