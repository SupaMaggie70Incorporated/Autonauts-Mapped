using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : InventoryBar
{
	private RenderTexture m_RenderTexture;

	private Texture2D m_FinalTexture;

	private GameObject m_FarmerModel;

	private Animator m_Animator;

	private BaseText m_Name;

	public override void SetObject(BaseClass NewObject)
	{
		base.SetObject(NewObject);
		m_Name = base.transform.Find("Panel").Find("TitleStrip/Title").GetComponent<BaseText>();
		BaseButtonBack component = base.transform.Find("Panel").Find("BackButton").GetComponent<BaseButtonBack>();
		component.SetAction(OnBackClicked, component);
		CheckTargetUpdated(m_Object);
		if (NewObject.m_TypeIdentifier == ObjectType.Wardrobe || Aquarium.GetIsTypeAquiarium(NewObject.m_TypeIdentifier))
		{
			base.transform.Find("HatSlot").gameObject.SetActive(value: false);
			base.transform.Find("TopSlot").gameObject.SetActive(value: false);
		}
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (m_RenderTexture != null)
		{
			Object.Destroy(m_RenderTexture);
		}
		if (m_FinalTexture != null)
		{
			Object.Destroy(m_FinalTexture);
		}
		DestroyModel();
	}

	public void OnBackClicked(BaseGadget NewGadget)
	{
		if (GameStateManager.Instance.GetActualState() == GameStateManager.State.Inventory)
		{
			GameStateManager.Instance.GetCurrentState().GetComponent<GameStateInventory>().Close();
		}
	}

	private void DestroyModel()
	{
		if ((bool)m_FarmerModel)
		{
			Object.Destroy(m_FarmerModel.gameObject);
			m_FarmerModel = null;
		}
	}

	private void UpdateFarmer()
	{
		DestroyModel();
		m_FarmerModel = Object.Instantiate(m_Object.m_ModelRoot);
		m_FarmerModel.transform.SetParent(null);
		if (m_Farmer != null)
		{
			Transform transform = m_FarmerModel.transform;
			if (m_Object.m_TypeIdentifier == ObjectType.Worker)
			{
				transform = transform.Find("Frame");
			}
			GameObject gameObject = transform.Find("CarryPoint").gameObject;
			if (gameObject.transform.childCount == 0 && (bool)transform.Find("ToolCarryPoint"))
			{
				gameObject = transform.Find("ToolCarryPoint").gameObject;
			}
			List<Transform> list = new List<Transform>();
			if (gameObject.transform.childCount != 0)
			{
				for (int i = 0; i < gameObject.transform.childCount; i++)
				{
					Transform child = gameObject.transform.GetChild(i);
					child.SetParent(null);
					list.Add(child);
				}
			}
			ObjectUtils.ObjectBounds(m_FarmerModel);
			foreach (Transform item in list)
			{
				item.transform.SetParent(gameObject.transform);
			}
		}
		m_FarmerModel.SetActive(value: false);
		DoRenderTexture();
	}

	private void DoRenderTexture()
	{
		GameObject gameObject = base.transform.Find("ModelPanel").gameObject;
		Rect rect = gameObject.GetComponent<RectTransform>().rect;
		Camera renderCamera = HudManager.Instance.m_RenderCamera;
		renderCamera.gameObject.SetActive(value: true);
		int num = (int)rect.width;
		int num2 = (int)rect.height;
		renderCamera.transform.position = default(Vector3);
		renderCamera.aspect = 1f;
		Bounds bounds = ObjectUtils.ObjectBoundsFromVerticesInCameraSpace(m_FarmerModel, renderCamera);
		float num3 = 0f;
		num3 = ((!(bounds.size.x > bounds.size.y)) ? (1f / bounds.size.y) : (1f / bounds.size.x));
		num3 *= 0.95f;
		float x = bounds.center.x;
		float y = bounds.center.y;
		float z = bounds.center.z;
		Vector3 vector = renderCamera.ViewportToWorldPoint(new Vector3(x, y, z)) * (0f - num3);
		Vector3 vector2 = new Vector3(-100f, 0f, 0f);
		m_FarmerModel.transform.localPosition = vector2 + vector;
		m_FarmerModel.transform.rotation = Quaternion.identity;
		m_FarmerModel.transform.localScale = new Vector3(num3, num3, num3);
		vector2 += renderCamera.transform.TransformPoint(new Vector3(0f, 0f, 10f));
		renderCamera.transform.position = vector2;
		if (m_RenderTexture == null)
		{
			m_RenderTexture = new RenderTexture(num, num2, 24);
			m_FinalTexture = new Texture2D(num, num2, TextureFormat.RGB24, mipChain: false);
			gameObject.GetComponent<RawImage>().texture = m_FinalTexture;
		}
		renderCamera.targetTexture = m_RenderTexture;
		CameraManager.Instance.RestorePausedDOFEffect();
		RenderTexture.active = m_RenderTexture;
		m_FarmerModel.SetActive(value: true);
		renderCamera.Render();
		m_FinalTexture.ReadPixels(new Rect(0f, 0f, num, num2), 0, 0);
		m_FinalTexture.Apply();
		CameraManager.Instance.SetPausedDOFEffect();
		renderCamera.targetTexture = null;
		RenderTexture.active = null;
		renderCamera.gameObject.SetActive(value: false);
		m_FarmerModel.SetActive(value: false);
	}

	private void UpdateName()
	{
		if (m_Farmer != null)
		{
			if ((bool)m_Farmer.GetComponent<FarmerPlayer>())
			{
				m_Name.SetTextFromID("FarmerPlayer");
			}
			else
			{
				m_Name.SetText(m_Farmer.GetHumanReadableName());
			}
		}
		else
		{
			m_Name.SetText(m_Object.GetHumanReadableName());
		}
	}

	public void CheckTargetUpdated(BaseClass Target)
	{
		if (Target == m_Object)
		{
			UpdateFarmer();
			UpdateName();
		}
	}
}
