using System;
using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections;

namespace GoogleMobileAds.Sample
{
    /// <summary>
    /// Demonstrates how to use Google Mobile Ads banner views.
    /// </summary>
    [AddComponentMenu("GoogleMobileAds/Samples/BannerViewController")]
    public class BannerViewController : AdController
    {

        // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
        private const string _adUnitId = "ca-app-pub-4025973742418082/6948335152";
        //private const string _adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        private const string _adUnitId = "unused";
#endif

        private BannerView bannerView;

        private void Start()
        {
            LoadAd();
        }

        /// <summary>
        /// Creates a 320x50 banner at top of the screen.
        /// </summary>
        public void CreateBannerView()
        {
            //Debug.Log("Creating banner view.");

            // If we already have a banner, destroy the old one.
            if(bannerView != null)
            {
                DestroyAd();
            }

            // Create a 320x50 banner at top of the screen.
            bannerView = new BannerView(_adUnitId, AdSize.Banner, AdPosition.Top);

            // Listen to events the banner may raise.
            ListenToAdEvents();

            //Debug.Log("Banner view created.");
        }

        /// <summary>
        /// Creates the banner view and loads a banner ad.
        /// </summary>
        public void LoadAd()
        {
            // Create an instance of a banner view first.
            if(bannerView == null)
            {
                CreateBannerView();
            }

            // Create our request used to load the ad.
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            //Debug.Log("Loading banner ad.");
            bannerView.LoadAd(adRequest);
        }

        /// <summary>
        /// Shows the ad.
        /// </summary>
        public void ShowAd()
        {
            if (bannerView != null)
            {
                //Debug.Log("Showing banner view.");
                bannerView.Show();
            }
        }

        /// <summary>
        /// Hides the ad.
        /// </summary>
        public void HideAd()
        {
            if (bannerView != null)
            {
                //Debug.Log("Hiding banner view.");
                bannerView.Hide();
            }
        }

        /// <summary>
        /// Destroys the ad.
        /// When you are finished with a BannerView, make sure to call
        /// the Destroy() method before dropping your reference to it.
        /// </summary>
        public void DestroyAd()
        {
            if (bannerView != null)
            {
                //Debug.Log("Destroying banner view.");
                bannerView.Destroy();
                bannerView = null;
            }

        }

        /// <summary>
        /// Logs the ResponseInfo.
        /// </summary>
        public void LogResponseInfo()
        {
            if (bannerView != null)
            {
                var responseInfo = bannerView.GetResponseInfo();
                if (responseInfo != null)
                {
                    Debug.Log(responseInfo);
                }
            }
        }

        /// <summary>
        /// Listen to events the banner may raise.
        /// </summary>
        private void ListenToAdEvents()
        {
            // Raised when an ad is loaded into the banner view.
            bannerView.OnBannerAdLoaded += () =>
            {

                /*Debug.Log("Banner view loaded an ad with response : "
                    + _bannerView.GetResponseInfo());*/
                isAvailable = true;
                ShowAd();
            };
            // Raised when an ad fails to load into the banner view.
            bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                //Debug.LogError("Banner view failed to load an ad with error : " + error);
            };
            // Raised when the ad is estimated to have earned money.
            bannerView.OnAdPaid += (AdValue adValue) =>
            {
                /*Debug.Log(String.Format("Banner view paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));*/
            };
            // Raised when an impression is recorded for an ad.
            bannerView.OnAdImpressionRecorded += () =>
            {
                //Debug.Log("Banner view recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            bannerView.OnAdClicked += () =>
            {
                SetAdClickRegister();
                DestroyAd();
                StartCoroutine(SetIsAdClicked());
                //Debug.Log("Banner view was clicked.");
            };
            // Raised when an ad opened full screen content.
            bannerView.OnAdFullScreenContentOpened += () =>
            {
                //Debug.Log("Banner view full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            bannerView.OnAdFullScreenContentClosed += () =>
            {
                //Debug.Log("Banner view full screen content closed.");
            };
        }

        protected override IEnumerator SetIsAdClicked()
        {
            while (isAdClicked)
            {
                currentDate = DateTime.Now.ToString("yyyyMMdd");
                if (string.IsNullOrEmpty(simulatedData))
                    adClickDate = PlayerPrefs.GetString(adTypeClickDate);
                else
                    adClickDate = simulatedData;

                if (currentDate != adClickDate)
                {
                    isAdClicked = false;
                    simulatedData = "";
                    LoadAd();
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
