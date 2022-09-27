using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : StandardButtonBlueprint
{
	private BuildingPalette m_Parent;

	[HideInInspector]
	public ObjectType m_Building;

	private bool m_Available;

	private Image m_BuiltCountCircle;

	private BaseText m_BuiltCount;

	private LevelHeart m_LevelHeart;

	private BaseImage m_Walls;

	private BaseImage m_Floors;

	private BaseImage m_Power;

	protected new void Awake()
	{
		base.Awake();
	}

	public void SetInfo(BuildingPalette Parent, ObjectType NewBuilding, bool New)
	{
		m_Parent = Parent;
		m_Building = NewBuilding;
		SetAction(OnClicked, this);
		SetObjectType(NewBuilding);
		m_BuiltCountCircle = base.transform.Find("BuiltCountCircle").GetComponent<Image>();
		m_BuiltCount = base.transform.Find("BuiltCount").GetComponent<BaseText>();
		m_LevelHeart = base.transform.Find("LevelHeart").GetComponent<LevelHeart>();
		GameObject gameObject = base.transform.Find("RequirementsPanel").gameObject;
		int variableAsInt = VariableManager.Instance.GetVariableAsInt(NewBuilding, "Floors", CheckValid: false);
		int variableAsInt2 = VariableManager.Instance.GetVariableAsInt(NewBuilding, "Walls", CheckValid: false);
		bool flag = LinkedSystemConverter.GetIsTypeLinkedSystemConverter(NewBuilding) || LinkedSystemEngine.GetIsTypeLinkedSystemEngine(m_Building) || m_Building == ObjectType.BeltLinkage;
		if (variableAsInt != 0 || variableAsInt2 != 0 || flag)
		{
			gameObject.SetActive(value: true);
			m_Walls = gameObject.transform.Find("Walls").GetComponent<BaseImage>();
			m_Floors = gameObject.transform.Find("Floors").GetComponent<BaseImage>();
			m_Power = gameObject.transform.Find("Power").GetComponent<BaseImage>();
			m_Floors.SetActive(variableAsInt != 0);
			m_Walls.SetActive(variableAsInt2 != 0);
			m_Power.SetActive(flag);
		}
		else
		{
			gameObject.SetActive(value: false);
		}
		UpdateHeartCount();
		UpdateBuiltCount();
		UpdateButtonUsable(ForceUpdate: true);
		if (New)
		{
			GameObject original = (GameObject)Resources.Load("Prefabs/Hud/NewThing", typeof(GameObject));
			m_NewIcon = Object.Instantiate(original, default(Vector3), Quaternion.identity, base.transform).GetComponent<NewThing>();
			m_NewIcon.GetComponent<RectTransform>().localPosition = new Vector3(-20f, 20f, 0f);
		}
	}

	public void UpdateButtonUsable(bool ForceUpdate = false)
	{
		bool flag = true;
		if (GameStateEdit.GetIsSingleBuildingType(m_Building) && GameStateEdit.GetSingleBuildingTypeExists(m_Building))
		{
			flag = false;
		}
		if (m_Available != flag || ForceUpdate)
		{
			m_Available = flag;
			SetInteractable(m_Available);
		}
	}

	public void UpdateBuiltCount()
	{
		int resource = ResourceManager.Instance.GetResource(m_Building);
		SetBuiltCount(resource);
	}

	private void UpdateHeartCount()
	{
		if (!Housing.GetIsTypeHouse(m_Building))
		{
			m_LevelHeart.SetActive(Active: false);
			return;
		}
		m_LevelHeart.SetActive(Active: true);
		int tierFromType = BaseClass.GetTierFromType(m_Building);
		m_LevelHeart.SetValue(tierFromType);
	}

	public void SetBuiltCount(int Count)
	{
		string backingSprite;
		if (Count == 0)
		{
			m_BuiltCountCircle.gameObject.SetActive(value: false);
			m_BuiltCount.SetActive(Active: false);
			backingSprite = "Buttons/BlueprintButton";
		}
		else
		{
			m_BuiltCountCircle.gameObject.SetActive(value: true);
			m_BuiltCount.SetActive(Active: true);
			m_BuiltCount.SetText(Count.ToString());
			backingSprite = "Panels/GeneralPanelBack";
		}
		SetBackingSprite(backingSprite);
	}

	public override void SetSelected(bool Selected)
	{
		base.SetSelected(Selected);
		UpdateButtonUsable();
	}

	public void OnClicked(BaseGadget NewGadget)
	{
		if (m_Available)
		{
			if (m_NewIcon != null)
			{
				m_NewIcon.gameObject.SetActive(value: false);
			}
			AudioManager.Instance.StartEvent("UIOptionSelected");
			m_Parent.SetBuilding(this);
			QuestManager.Instance.AddEvent(QuestEvent.Type.SelectBlueprint, Bot: false, m_Type, null);
		}
	}

	public override void SetInteractable(bool Interactable)
	{
		base.SetInteractable(Interactable);
		BaseSetInteractable(Interactable);
	}
}
