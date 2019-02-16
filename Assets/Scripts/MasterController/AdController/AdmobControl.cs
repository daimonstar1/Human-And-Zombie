using System;
using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System.Collections.Generic;

public class AdmobControl : SingletonMono<AdmobControl>
{
	public string bannerAdmobID_Android;
	public string InadsAdmobID_Android;
	public string videoAdmobID_Android;
	public string bannerAdmobID_IOS;
	public string InadsAdmobID_IOS;
	public string videoAdmobID_IOS;
	string bannerAdmobID, InadsAdmobID;
	public InterstitialAd interstitial;
	private RewardBasedVideoAd rewardBasedVideo;
	public bool isTest;
	BannerView bannerView;

	public static AdmobControl mark;

	void Awake ()
	{
		if (mark == null) {
			mark = this;
			DontDestroyOnLoad (gameObject);
		} else
			Destroy (gameObject);
		if (isTest == true) {
			bannerAdmobID = "ca-app-pub-3940256099942544/6300978111";
			InadsAdmobID = "ca-app-pub-3940256099942544/1033173712";
		} else {
			#if UNITY_ANDROID
			bannerAdmobID = bannerAdmobID_Android;
			InadsAdmobID = InadsAdmobID_Android;
			#elif UNITY_IOS
      bannerAdmobID = bannerAdmobID_IOS;
      InadsAdmobID = InadsAdmobID_IOS;
			#endif
		}
		rewardBasedVideo = RewardBasedVideoAd.Instance;

		// Called when an ad request has successfully loaded.
		rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
		// Called when an ad request failed to load.
		rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
		// Called when the ad is closed.
		rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
		RequestRewardBasedVideo ();
	}

	void Start ()
	{
		RequestInterstitial ();
	}

	public void HandleRewardBasedVideoLoaded (object sender, EventArgs args)
	{
		MonoBehaviour.print ("HandleRewardBasedVideoLoaded event received");
	}

	public void HandleRewardBasedVideoFailedToLoad (object sender, AdFailedToLoadEventArgs args)
	{
		MonoBehaviour.print (
			"HandleRewardBasedVideoFailedToLoad event received with message: "
			+ args.Message);
	}

	public void HandleRewardBasedVideoClosed (object sender, EventArgs args)
	{
		MonoBehaviour.print ("HandleRewardBasedVideoClosed event received");
		if (_FreeRewardDialog != null) {
			if (type == 0)
				_FreeRewardDialog.showVideoCompleted (true);
			else
				_FreeRewardDialog.callback1 (true);
			_FreeRewardDialog = null;
		}
	}

	private void RequestRewardBasedVideo ()
	{
		#if UNITY_ANDROID
		string adUnitId = videoAdmobID_Android;
		#elif UNITY_IPHONE
    string adUnitId = videoAdmobID_Android;
		#else
    string adUnitId = "unexpected_platform";
		#endif

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder ().Build ();
		// Load the rewarded video ad with the request.
		this.rewardBasedVideo.LoadAd (request, adUnitId);
	}

	public bool isAdmobVideoAvailable ()
	{
		return rewardBasedVideo.IsLoaded ();
	}

	private FreeRewardDialog _FreeRewardDialog;
	private int type = -1;

	public void showAdmobVideo (FreeRewardDialog _FreeRewardDialog, int type)
	{
		Debug.LogError ("showAdmobVideo");
		this.type = type;
		this._FreeRewardDialog = _FreeRewardDialog;
		#if UNITY_EDITOR
		if (_FreeRewardDialog != null) {
			if (type == 0)
				_FreeRewardDialog.showVideoCompleted (true);
			else
				_FreeRewardDialog.callback1 (true);
		}
		#else
    if (rewardBasedVideo.IsLoaded ())
      rewardBasedVideo.Show ();
    else if (_FreeRewardDialog != null)
    {
    if (type == 0)
    _FreeRewardDialog.showVideoCompleted (false);
    else
    _FreeRewardDialog.callback1 (false);
    }
		#endif
	}

	public void RequestBannerAdmob ()
	{
		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView (bannerAdmobID, AdSize.Banner, AdPosition.Bottom);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder ().Build ();
		// Load the banner with the request.
		bannerView.LoadAd (request);
		bannerView.Hide ();
	}

	public bool isHasBanner ()
	{
		if (bannerView != null) {
			return true;
		}
		return false;
	}

	public void ShowBannerAdmob ()
	{
		if (bannerView != null) {
			bannerView.Show ();
		}
	}

	public void HideBannerAdmob ()
	{
		if (bannerView != null) {
			bannerView.Hide ();
		}
	}

	public void ClearBannerAdmob ()
	{
		if (bannerView != null) {
			bannerView.Hide ();
			bannerView.Destroy ();
		}
	}

	public void RequestInterstitial ()
	{
		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd (InadsAdmobID);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder ().Build ();
		this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
		interstitial.LoadAd (request);
	}

	public void ShowInadsAdmob ()
	{
		Debug.LogError ("Show ads interstitial!");
		if (interstitial != null && interstitial.IsLoaded ()) {
			interstitial.Show ();
		} else if (interstitial == null)
			Debug.LogError ("interstitial=null");
		else
			Debug.LogError ("interstitial.IsLoaded=" + interstitial.IsLoaded ());
	}

	public void ClearInadsAdmob ()
	{
		if (interstitial != null) {
			interstitial.Destroy ();
		}
	}

	public void HandleInterstitialClosed (object sender, EventArgs args)
	{

	}
}
