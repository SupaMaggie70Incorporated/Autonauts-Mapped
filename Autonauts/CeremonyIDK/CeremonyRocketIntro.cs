using UnityEngine;

public class CeremonyRocketIntro : CeremonyBase
{
	public static CeremonyRocketIntro Instance;

	private bool m_Finished;

	private Fader m_Fader;

	private BaseText m_NumberText;

	private BaseText m_NameText;

	private RocketAnimation m_RocketAnimation;

	private void Awake()
	{
		Instance = this;
		m_Finished = false;
		TileCoord tileCoord = CollectionManager.Instance.GetPlayers()[0].GetComponent<FarmerPlayer>().m_TileCoord + Transmitter.m_StartingOffsetFromPlayer;
		m_RocketAnimation = ObjectTypeList.Instance.CreateObjectFromIdentifier(ObjectType.RocketAnimation, tileCoord.ToWorldPositionTileCentered(), Quaternion.identity).GetComponent<RocketAnimation>();
		GameObject original = (GameObject)Resources.Load("Prefabs/Hud/Fader", typeof(GameObject));
		m_Fader = Object.Instantiate(original, new Vector3(0f, 0f, 0f), Quaternion.identity, HudManager.Instance.m_SaveImageRootTransform).GetComponent<Fader>();
		m_Fader.StartFade(Up: false, 1f, new Color(1f, 1f, 1f, 1f));
		m_NumberText = base.transform.Find("PlanetText").GetComponent<BaseText>();
		string mapSeedString = GeneralUtils.GetMapSeedString(GameOptionsManager.Instance.m_Options.m_MapSeed);
		string text = TextManager.Instance.Get("CeremonyRocketIntro", mapSeedString);
		m_NumberText.SetText(text);
		m_NumberText.SetActive(Active: false);
		m_NameText = base.transform.Find("NameText").GetComponent<BaseText>();
		text = "\"" + GameOptionsManager.Instance.m_Options.m_MapName + "\"";
		m_NameText.SetText(text);
		m_NameText.SetActive(Active: false);
	}

	private void OnDestroy()
	{
		DestroyAll();
	}

	private void DestroyAll()
	{
		RocketFinished();
		if ((bool)m_Fader)
		{
			Object.Destroy(m_Fader.gameObject);
			m_Fader = null;
		}
	}

	public override void Skip()
	{
		m_RocketAnimation.Skip();
		DestroyAll();
	}

	public void FadeTextIn()
	{
		m_NumberText.SetActive(Active: true);
	}

	public void ShowName()
	{
		m_NameText.SetActive(Active: true);
	}

	public void FadeTextOut()
	{
		m_NumberText.SetActive(Active: false);
		m_NameText.SetActive(Active: false);
	}

	public void RocketFinished()
	{
		if ((bool)m_NumberText)
		{
			Object.Destroy(m_NumberText.gameObject);
			m_NumberText = null;
		}
		if ((bool)m_NameText)
		{
			Object.Destroy(m_NameText.gameObject);
			m_NameText = null;
		}
		m_Finished = true;
		QuestManager.Instance.AddEvent(QuestEvent.Type.Land, Bot: false, 0, null);
	}

	private void Update()
	{
		if (m_Finished)
		{
			Object.Destroy(base.gameObject);
			CeremonyManager.Instance.CeremonyEnded();
		}
		else if (!QuestManager.Instance.gameObject.activeSelf)
		{
			QuestManager.Instance.gameObject.SetActive(value: true);
		}
	}
}
