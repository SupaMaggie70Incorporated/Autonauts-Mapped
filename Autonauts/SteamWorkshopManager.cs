using System.Collections.Generic;
using Steamworks;
using UnityEngine;

[DisallowMultipleComponent]
public class SteamWorkshopManager : MonoBehaviour
{
	public enum WorkshopSearchState
	{
		Not_Searching,
		Searching_Subscribed_Items,
		Searching_User_Items,
		Search_Subscribed_Items_Complete,
		Search_User_Items_Complete
	}

	private Vector2 m_ScrollPos;

	private Mod CurrentMod;

	public bool NeedsMenuRefresh;

	public List<SteamUGCDetails_t> Details;

	private UGCQueryHandle_t m_UGCQueryHandle;

	public UGCUpdateHandle_t m_UGCUpdateHandle;

	protected Callback<ItemInstalled_t> m_ItemInstalled;

	protected Callback<DownloadItemResult_t> m_DownloadItemResult;

	private CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryCompletedCallResult;

	private CallResult<SteamUGCRequestUGCDetailsResult_t> OnSteamUGCRequestUGCDetailsResultCallResult;

	private CallResult<CreateItemResult_t> OnCreateItemResultCallResult;

	private CallResult<SubmitItemUpdateResult_t> OnSubmitItemUpdateResultCallResult;

	private CallResult<UserFavoriteItemsListChanged_t> OnUserFavoriteItemsListChangedCallResult;

	private CallResult<SetUserItemVoteResult_t> OnSetUserItemVoteResultCallResult;

	private CallResult<GetUserItemVoteResult_t> OnGetUserItemVoteResultCallResult;

	private CallResult<StartPlaytimeTrackingResult_t> OnStartPlaytimeTrackingResultCallResult;

	private CallResult<StopPlaytimeTrackingResult_t> OnStopPlaytimeTrackingResultCallResult;

	private CallResult<AddUGCDependencyResult_t> OnAddUGCDependencyResultCallResult;

	private CallResult<RemoveUGCDependencyResult_t> OnRemoveUGCDependencyResultCallResult;

	private CallResult<AddAppDependencyResult_t> OnAddAppDependencyResultCallResult;

	private CallResult<RemoveAppDependencyResult_t> OnRemoveAppDependencyResultCallResult;

	private CallResult<GetAppDependenciesResult_t> OnGetAppDependenciesResultCallResult;

	private CallResult<DeleteItemResult_t> OnDeleteItemResultCallResult;

	private static SteamWorkshopManager s_instance;

	private CallResult<RemoteStorageSubscribePublishedFileResult_t> OnRemoteStorageSubscribePublishedFileResultCallResult;

	private CallResult<RemoteStorageUnsubscribePublishedFileResult_t> OnRemoteStorageUnsubscribePublishedFileResultCallResult;

	public WorkshopSearchState SearchStateUserItems { get; private set; }

	public WorkshopSearchState SearchStateSubscribedItems { get; private set; }

	public static SteamWorkshopManager Instance
	{
		get
		{
			if (s_instance == null)
			{
				return new GameObject("SteamWorkshopManager").AddComponent<SteamWorkshopManager>();
			}
			return s_instance;
		}
	}

	public static bool Initialized => SteamManager.Initialized;

	private void Awake()
	{
		if (s_instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		s_instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void OnEnable()
	{
		if (SteamManager.Initialized)
		{
			OnRemoteStorageSubscribePublishedFileResultCallResult = CallResult<RemoteStorageSubscribePublishedFileResult_t>.Create(OnRemoteStorageSubscribePublishedFileResult);
			OnRemoteStorageUnsubscribePublishedFileResultCallResult = CallResult<RemoteStorageUnsubscribePublishedFileResult_t>.Create(OnRemoteStorageUnsubscribePublishedFileResult);
			m_ItemInstalled = Callback<ItemInstalled_t>.Create(OnItemInstalled);
			m_DownloadItemResult = Callback<DownloadItemResult_t>.Create(OnDownloadItemResult);
			OnSteamUGCQueryCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(OnSteamUGCQueryCompleted);
			OnSteamUGCRequestUGCDetailsResultCallResult = CallResult<SteamUGCRequestUGCDetailsResult_t>.Create(OnSteamUGCRequestUGCDetailsResult);
			OnCreateItemResultCallResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResult);
			OnSubmitItemUpdateResultCallResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemUpdateResult);
			OnUserFavoriteItemsListChangedCallResult = CallResult<UserFavoriteItemsListChanged_t>.Create(OnUserFavoriteItemsListChanged);
			OnSetUserItemVoteResultCallResult = CallResult<SetUserItemVoteResult_t>.Create(OnSetUserItemVoteResult);
			OnGetUserItemVoteResultCallResult = CallResult<GetUserItemVoteResult_t>.Create(OnGetUserItemVoteResult);
			OnStartPlaytimeTrackingResultCallResult = CallResult<StartPlaytimeTrackingResult_t>.Create(OnStartPlaytimeTrackingResult);
			OnStopPlaytimeTrackingResultCallResult = CallResult<StopPlaytimeTrackingResult_t>.Create(OnStopPlaytimeTrackingResult);
			OnAddUGCDependencyResultCallResult = CallResult<AddUGCDependencyResult_t>.Create(OnAddUGCDependencyResult);
			OnRemoveUGCDependencyResultCallResult = CallResult<RemoveUGCDependencyResult_t>.Create(OnRemoveUGCDependencyResult);
			OnAddAppDependencyResultCallResult = CallResult<AddAppDependencyResult_t>.Create(OnAddAppDependencyResult);
			OnRemoveAppDependencyResultCallResult = CallResult<RemoveAppDependencyResult_t>.Create(OnRemoveAppDependencyResult);
			OnGetAppDependenciesResultCallResult = CallResult<GetAppDependenciesResult_t>.Create(OnGetAppDependenciesResult);
			OnDeleteItemResultCallResult = CallResult<DeleteItemResult_t>.Create(OnDeleteItemResult);
			Details = new List<SteamUGCDetails_t>();
			SearchStateSubscribedItems = WorkshopSearchState.Not_Searching;
			SearchStateUserItems = WorkshopSearchState.Not_Searching;
		}
	}

	private void OnRemoteStorageSubscribePublishedFileResult(RemoteStorageSubscribePublishedFileResult_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult != EResult.k_EResultOK)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_FailedSubcribe, pCallback.m_eResult);
			return;
		}
		if (ModManager.Instance.CurrentErrorState == ModManager.ErrorState.No_Error)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Restart);
		}
		NeedsMenuRefresh = true;
	}

	private void OnRemoteStorageUnsubscribePublishedFileResult(RemoteStorageUnsubscribePublishedFileResult_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult != EResult.k_EResultOK)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_FailedUnsubcribe, pCallback.m_eResult);
			return;
		}
		if (ModManager.Instance.CurrentErrorState == ModManager.ErrorState.No_Error)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Restart);
		}
		NeedsMenuRefresh = true;
	}

	private void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t pCallback, bool bIOFailure)
	{
		Debug.Log(string.Concat("[", 3401, " - SteamUGCQueryCompleted] - ", pCallback.m_handle, " -- ", pCallback.m_eResult, " -- ", pCallback.m_unNumResultsReturned, " -- ", pCallback.m_unTotalMatchingResults, " -- ", pCallback.m_bCachedData.ToString()));
		if (pCallback.m_eResult != EResult.k_EResultOK)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_FailedResults, pCallback.m_eResult);
			return;
		}
		Details.Clear();
		for (int i = 0; i < pCallback.m_unNumResultsReturned; i++)
		{
			if (SteamUGC.GetQueryUGCResult(m_UGCQueryHandle, (uint)i, out var pDetails))
			{
				Details.Add(pDetails);
				Debug.Log(string.Concat(pDetails.m_nPublishedFileId, " -- ", pDetails.m_eResult, " -- ", pDetails.m_eFileType, " -- ", pDetails.m_nCreatorAppID, " -- ", pDetails.m_nConsumerAppID, " -- ", pDetails.m_rgchTitle, " -- ", pDetails.m_rgchDescription, " -- ", pDetails.m_ulSteamIDOwner, " -- ", pDetails.m_rtimeCreated, " -- ", pDetails.m_rtimeUpdated, " -- ", pDetails.m_rtimeAddedToUserList, " -- ", pDetails.m_eVisibility, " -- ", pDetails.m_bBanned.ToString(), " -- ", pDetails.m_bAcceptedForUse.ToString(), " -- ", pDetails.m_bTagsTruncated.ToString(), " -- ", pDetails.m_rgchTags, " -- ", pDetails.m_hFile, " -- ", pDetails.m_hPreviewFile, " -- ", pDetails.m_pchFileName, " -- ", pDetails.m_nFileSize, " -- ", pDetails.m_nPreviewFileSize, " -- ", pDetails.m_rgchURL, " -- ", pDetails.m_unVotesUp, " -- ", pDetails.m_unVotesDown, " -- ", pDetails.m_flScore, " -- ", pDetails.m_unNumChildren));
			}
		}
		SteamUGC.ReleaseQueryUGCRequest(m_UGCQueryHandle);
		if (SearchStateUserItems == WorkshopSearchState.Searching_User_Items)
		{
			bool flag = false;
			PublishedFileId_t publishedFileId_t = new PublishedFileId_t(0uL);
			foreach (SteamUGCDetails_t detail in Details)
			{
				Debug.Log(detail.m_ulSteamIDOwner + ": SEARCHING FOUND ID = " + detail.m_nPublishedFileId);
				if (CurrentMod.m_PublishedFileId == detail.m_nPublishedFileId)
				{
					publishedFileId_t = detail.m_nPublishedFileId;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				Debug.Log("MOD FOUND ID = " + publishedFileId_t);
				CurrentMod.SetPublishedFieldID(publishedFileId_t);
				CreateWorkshopItemStartUpload(publishedFileId_t);
			}
			else
			{
				Debug.Log("MOD NOT FOUND  - NEW ID REQUIRED");
				SteamAPICall_t hAPICall = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeFirst);
				OnCreateItemResultCallResult.Set(hAPICall);
			}
		}
		if (SearchStateUserItems == WorkshopSearchState.Searching_User_Items)
		{
			SearchStateUserItems = WorkshopSearchState.Search_User_Items_Complete;
		}
		if (SearchStateSubscribedItems == WorkshopSearchState.Searching_Subscribed_Items)
		{
			SearchStateSubscribedItems = WorkshopSearchState.Search_Subscribed_Items_Complete;
		}
	}

	private void OnSteamUGCRequestUGCDetailsResult(SteamUGCRequestUGCDetailsResult_t pCallback, bool bIOFailure)
	{
	}

	private void OnCreateItemResult(CreateItemResult_t pCallback, bool bIOFailure)
	{
		Debug.Log(string.Concat("[", 3403, " - CreateItemResult] - ", pCallback.m_eResult, " -- ", pCallback.m_nPublishedFileId, " -- ", pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement.ToString()));
		if (pCallback.m_eResult != EResult.k_EResultOK)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Upload_Steam, pCallback.m_eResult);
			SteamAPICall_t hAPICall = SteamUGC.DeleteItem(CurrentMod.m_PublishedFileId);
			OnDeleteItemResultCallResult.Set(hAPICall);
		}
		else if (pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_AcceptTCs);
			SteamAPICall_t hAPICall2 = SteamUGC.DeleteItem(CurrentMod.m_PublishedFileId);
			OnDeleteItemResultCallResult.Set(hAPICall2);
		}
		else
		{
			CurrentMod.SetPublishedFieldID(pCallback.m_nPublishedFileId);
			CreateWorkshopItemStartUpload(pCallback.m_nPublishedFileId);
		}
	}

	private void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult != EResult.k_EResultOK)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Upload_Steam, pCallback.m_eResult);
			return;
		}
		SteamFriends.ActivateGameOverlayToWebPage("steam://url/CommunityFilePage/" + pCallback.m_nPublishedFileId);
		GameStateModsUploadConfirm component = GameStateManager.Instance.GetCurrentState().GetComponent<GameStateModsUploadConfirm>();
		if ((bool)component)
		{
			component.SetUploadComplete();
		}
	}

	private void OnItemInstalled(ItemInstalled_t pCallback)
	{
		if (ModManager.Instance.CurrentErrorState == ModManager.ErrorState.No_Error)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Restart);
		}
		NeedsMenuRefresh = true;
	}

	private void OnDownloadItemResult(DownloadItemResult_t pCallback)
	{
	}

	private void OnUserFavoriteItemsListChanged(UserFavoriteItemsListChanged_t pCallback, bool bIOFailure)
	{
	}

	private void OnSetUserItemVoteResult(SetUserItemVoteResult_t pCallback, bool bIOFailure)
	{
	}

	private void OnGetUserItemVoteResult(GetUserItemVoteResult_t pCallback, bool bIOFailure)
	{
	}

	private void OnStartPlaytimeTrackingResult(StartPlaytimeTrackingResult_t pCallback, bool bIOFailure)
	{
	}

	private void OnStopPlaytimeTrackingResult(StopPlaytimeTrackingResult_t pCallback, bool bIOFailure)
	{
	}

	private void OnAddUGCDependencyResult(AddUGCDependencyResult_t pCallback, bool bIOFailure)
	{
	}

	private void OnRemoveUGCDependencyResult(RemoveUGCDependencyResult_t pCallback, bool bIOFailure)
	{
	}

	private void OnAddAppDependencyResult(AddAppDependencyResult_t pCallback, bool bIOFailure)
	{
	}

	private void OnRemoveAppDependencyResult(RemoveAppDependencyResult_t pCallback, bool bIOFailure)
	{
	}

	private void OnGetAppDependenciesResult(GetAppDependenciesResult_t pCallback, bool bIOFailure)
	{
	}

	private void OnDeleteItemResult(DeleteItemResult_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_eResult == EResult.k_EResultOK)
		{
			NeedsMenuRefresh = true;
			if (ModManager.Instance.CurrentErrorState == ModManager.ErrorState.No_Error)
			{
				ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Restart);
			}
		}
	}

	public void CreateWorkshopItem(Mod InMod)
	{
		if (!SteamManager.Initialized)
		{
			Debug.LogError("STEAM NOT INITIALISED");
			return;
		}
		Debug.Log("Called Create Workshop Item");
		CurrentMod = InMod;
		FindUserItems();
	}

	private void CreateWorkshopItemStartUpload(PublishedFileId_t PublishID)
	{
		if (!SteamManager.Initialized)
		{
			Debug.LogError("STEAM NOT INITIALISED");
			return;
		}
		if (CurrentMod.SteamTitle == null)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Upload_Title);
			return;
		}
		if (CurrentMod.SteamTitle.Length == 0)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Upload_Title);
			return;
		}
		if (CurrentMod.SteamDescription.Length == 0)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Upload_Description);
			return;
		}
		if (CurrentMod.SteamTags.Count == 0)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Upload_Tags);
			return;
		}
		if (CurrentMod.SteamImageName.Length == 0 || CurrentMod.GetTexture(CurrentMod.SteamImageName) == null)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Upload_Image);
			return;
		}
		m_UGCUpdateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), PublishID);
		bool flag = SteamUGC.SetItemTitle(m_UGCUpdateHandle, CurrentMod.SteamTitle);
		bool flag2 = SteamUGC.SetItemDescription(m_UGCUpdateHandle, CurrentMod.SteamDescription);
		SteamUGC.SetItemUpdateLanguage(m_UGCUpdateHandle, "en");
		SteamUGC.SetItemMetadata(m_UGCUpdateHandle, CurrentMod.SteamDescription);
		SteamUGC.SetItemVisibility(m_UGCUpdateHandle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic);
		bool flag3 = SteamUGC.SetItemTags(m_UGCUpdateHandle, CurrentMod.SteamTags);
		SteamUGC.SetItemContent(m_UGCUpdateHandle, CurrentMod.SteamContentFolder);
		bool flag4 = SteamUGC.SetItemPreview(m_UGCUpdateHandle, CurrentMod.SteamContentImage);
		if (flag && flag2 && flag4 && flag3)
		{
			SteamAPICall_t hAPICall = SteamUGC.SubmitItemUpdate(m_UGCUpdateHandle, null);
			OnSubmitItemUpdateResultCallResult.Set(hAPICall);
		}
		else if (!flag)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Upload_Title);
		}
		else if (!flag2)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Upload_Description);
		}
		else if (!flag3)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Upload_Tags);
		}
		else if (!flag4)
		{
			ModManager.Instance.SetErrorSteam(ModManager.ErrorState.Error_Upload_Image);
		}
	}

	public void ShowSteamWorkshopOverlay()
	{
		if (!SteamManager.Initialized)
		{
			Debug.LogError("STEAM NOT INITIALISED");
		}
		else
		{
			SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/workshop/browse?appid=" + SteamUtils.GetAppID());
		}
	}

	public void UnsubscribeWorkshopItem(PublishedFileId_t PublishID)
	{
		if (!SteamManager.Initialized)
		{
			Debug.LogError("STEAM NOT INITIALISED");
			return;
		}
		SteamAPICall_t hAPICall = SteamUGC.UnsubscribeItem(PublishID);
		OnRemoteStorageUnsubscribePublishedFileResultCallResult.Set(hAPICall);
	}

	public void GetSubscribedItems()
	{
		if (!SteamManager.Initialized)
		{
			Debug.LogError("STEAM NOT INITIALISED");
			return;
		}
		m_UGCQueryHandle = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Subscribed, EUGCMatchingUGCType.k_EUGCMatchingUGCType_All, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, AppId_t.Invalid, SteamUtils.GetAppID(), 1u);
		SearchStateSubscribedItems = WorkshopSearchState.Searching_Subscribed_Items;
		SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(m_UGCQueryHandle);
		OnSteamUGCQueryCompletedCallResult.Set(hAPICall);
	}

	public string GetSubscribedItemFolderLocation(PublishedFileId_t PublishID)
	{
		if (!SteamManager.Initialized)
		{
			Debug.LogError("STEAM NOT INITIALISED");
			return "";
		}
		SteamUGC.GetItemInstallInfo(PublishID, out var _, out var pchFolder, 1024u, out var _);
		return pchFolder;
	}

	public bool GetSubscribedItemComplete(PublishedFileId_t PublishID)
	{
		if (!SteamManager.Initialized)
		{
			Debug.LogError("STEAM NOT INITIALISED");
			return false;
		}
		ulong punSizeOnDisk;
		string pchFolder;
		uint punTimeStamp;
		return SteamUGC.GetItemInstallInfo(PublishID, out punSizeOnDisk, out pchFolder, 1024u, out punTimeStamp);
	}

	public bool IsSubscribedItemDownloaded(PublishedFileId_t PublishID)
	{
		if (!SteamManager.Initialized)
		{
			Debug.LogError("STEAM NOT INITIALISED");
			return false;
		}
		uint itemState = SteamUGC.GetItemState(PublishID);
		if (itemState != 4 && itemState != 5 && itemState != 8)
		{
			return itemState == 9;
		}
		return true;
	}

	public void FindUserItems()
	{
		if (!SteamManager.Initialized)
		{
			Debug.LogError("STEAM NOT INITIALISED");
			return;
		}
		m_UGCQueryHandle = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_All, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, AppId_t.Invalid, SteamUtils.GetAppID(), 1u);
		SearchStateUserItems = WorkshopSearchState.Searching_User_Items;
		SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(m_UGCQueryHandle);
		OnSteamUGCQueryCompletedCallResult.Set(hAPICall);
	}
}
