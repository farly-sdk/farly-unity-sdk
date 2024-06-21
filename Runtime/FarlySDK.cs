using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FarlySDK
{
  public static class Farly
  {
    private const string GAME_OBJECT_NAME = "FarlyPluginBridge";
    private static GameObject gameObject;

    // Android only variables
    private const string JAVA_OBJECT_NAME = "com.farly.nativeandroidunity.FarlyNativeAndroidSDK";
    private static AndroidJavaObject androidJavaNative;

    // iOS only variables
#if UNITY_IOS
    [DllImport("__Internal")]
#endif
    private static extern void _configureFarly(string publisherId);
#if UNITY_IOS
    [DllImport("__Internal")]
#endif
    private static extern void _getHostedOfferwallUrlFarly(string requestJson);
#if UNITY_IOS
    [DllImport("__Internal")]
#endif
    private static extern void _showOfferwallInWebviewFarly(string requestJson);
#if UNITY_IOS
    [DllImport("__Internal")]
#endif
    private static extern void _showOfferwallInBrowserFarly(string requestJson);
#if UNITY_IOS
    [DllImport("__Internal")]
#endif
    private static extern void _getOfferwallFarly(string requestJson);

    // Save a reference of the callback to pass async messages
    private static Action<string> handleOfferWallUrlResponse;
    private static Action<FeedElement[]> handleOfferWallResponse;

    // Default error message
    private class PlatformNotSupportedException : Exception
    {
      public PlatformNotSupportedException() : base() { }
    }

    static Farly()
    {

      // Create Game Object to allow sending messages from Java or Objective C to Unity
      gameObject = new GameObject();
      // Object name must match UnitySendMessage call in Java or Objective C
      gameObject.name = GAME_OBJECT_NAME;
      // Attach this class to allow for handling of callbacks from Java or Objective C
      gameObject.AddComponent<NativeCallsCallbackHandler>();
      // Do not destroy when loading a new scene
      UnityEngine.Object.DontDestroyOnLoad(gameObject);

      // Initialize Plugin
      switch (Application.platform)
      {
        case RuntimePlatform.Android:
          // get the current android activity - this is needed to initialize the native Java object
          var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
          var currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
          // Initialize native Java object
          androidJavaNative = new AndroidJavaObject(JAVA_OBJECT_NAME, currentActivity);
          break;

        case RuntimePlatform.IPhonePlayer:
          // No initialization needed
          break;

        default:
          throw new PlatformNotSupportedException();
      }
    }

    private class NativeCallsCallbackHandler : MonoBehaviour
    {
      private void HandleException(string exception)
      {
        throw new Exception(exception);
      }

      private void HandleAsyncOfferwallUrlAnswer(string answerResultsJSON)
      {
        handleOfferWallUrlResponse?.Invoke(answerResultsJSON);
      }

      private void HandleAsyncOfferwallAnswer(string answerResultsJSON)
      {
        handleOfferWallResponse?.Invoke(JsonUtility.FromJson<FeedElements>("{\"items\":" + answerResultsJSON + "}").items); // hack because we cant deserialize an array directly
      }
    }

    public static string Configure(string publisherId)
    {
      switch (Application.platform)
      {
        case RuntimePlatform.Android:
          androidJavaNative.Call("configure", publisherId);
          return "ok";

        case RuntimePlatform.IPhonePlayer:
          _configureFarly(publisherId);
          return "ok";

        default:
          throw new PlatformNotSupportedException();
      }
    }

    public static void getHostedOfferwallUrl(OfferWallRequest request, Action<String> handleAsync)
    {
      if (string.IsNullOrEmpty(request.userId))
      {
        throw new ArgumentException("userId is required");
      }

      Farly.handleOfferWallUrlResponse = handleAsync;

      switch (Application.platform)
      {
        case RuntimePlatform.Android:
          androidJavaNative.Call("getHostedOfferwallUrl", JsonUtility.ToJson(new OfferWallSerializable(request)));
          break;

        case RuntimePlatform.IPhonePlayer:
          _getHostedOfferwallUrlFarly(JsonUtility.ToJson(new OfferWallSerializable(request)));
          break;

        default:
          throw new PlatformNotSupportedException();
      }
    }

    public static void showOfferwallInWebview(OfferWallRequest request)
    {
      if (string.IsNullOrEmpty(request.userId))
      {
        throw new ArgumentException("userId is required");
      }
      switch (Application.platform)
      {
        case RuntimePlatform.Android:
          androidJavaNative.Call("showOfferwallInWebview", JsonUtility.ToJson(new OfferWallSerializable(request)));
          return;

        case RuntimePlatform.IPhonePlayer:
          _showOfferwallInWebviewFarly(JsonUtility.ToJson(new OfferWallSerializable(request)));
          return;

        default:
          throw new PlatformNotSupportedException();
      }
    }

    public static void showOfferwallInBrowser(OfferWallRequest request)
    {
      if (string.IsNullOrEmpty(request.userId))
      {
        throw new ArgumentException("userId is required");
      }
      switch (Application.platform)
      {
        case RuntimePlatform.Android:
          androidJavaNative.Call("showOfferwallInBrowser", JsonUtility.ToJson(new OfferWallSerializable(request)));
          return;

        case RuntimePlatform.IPhonePlayer:
          _showOfferwallInBrowserFarly(JsonUtility.ToJson(new OfferWallSerializable(request)));
          return;

        default:
          throw new PlatformNotSupportedException();
      }
    }

    public static void getOfferwall(OfferWallRequest request, Action<FeedElement[]> handleAsync)
    {
      if (string.IsNullOrEmpty(request.userId))
      {
        throw new ArgumentException("userId is required");
      }

      Farly.handleOfferWallResponse = handleAsync;

      switch (Application.platform)
      {
        case RuntimePlatform.Android:
          androidJavaNative.Call("getOfferwall", JsonUtility.ToJson(new OfferWallSerializable(request)));
          break;

        case RuntimePlatform.IPhonePlayer:
          _getOfferwallFarly(JsonUtility.ToJson(new OfferWallSerializable(request)));
          break;

        default:
          throw new PlatformNotSupportedException();
      }
    }
  }


  public class OfferWallRequest
  {
    /// <summary>
    /// Your unique id for the current user
    /// </summary>
    public string userId;

    /// <summary>
    /// Current zipCode of the user, should be fetched from geolocation, not from geoip
    /// </summary>
    public string zipCode;

    /// <summary>
    /// Current 2 letters country code of the user,
    /// if not provided will default to the user's preferred region
    /// </summary>
    public string countryCode;

    /// <summary>
    /// Your user's age
    /// </summary>
    public int? userAge;

    /// <summary>
    /// Gender of the user, to access targetted campaigns (m or f)
    /// </summary>
    public string userGender;

    /// <summary>
    /// Date at which your user did signup.
    /// </summary>
    public DateTime? userSignupDateTime;

    /// <summary>
    /// parameters you wish to get back in your callback
    /// </summary>
    public string[] callbackParameters;
  }

  [Serializable]
  class OfferWallSerializable
  {
    public string userId;
    public string zipCode;
    public string countryCode;
    public int? userAge;
    public string userGender;
    public string userSignupDateTimestamp; // stored as string because DateTime is not serializable
    public string[] callbackParameters;
    // constructor to convert from OfferWallRequest to OfferWallSerializable
    public OfferWallSerializable(OfferWallRequest request)
    {
      userId = request.userId;
      zipCode = request.zipCode;
      countryCode = request.countryCode;
      userAge = request.userAge;
      userGender = request.userGender;
      // store the unix timestamp as a string
      if (request.userSignupDateTime != null)
      {
        userSignupDateTimestamp = request.userSignupDateTime.Value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds.ToString();
      }
      callbackParameters = request.callbackParameters;
    }
  }

  [Serializable]
  public class Action
  {
    public string id;
    public int amount;
    public string text;
    public string html;
  }

  [Serializable]
  public class FeedElement
  {
    public string id;
    public string name;
    public string devName;
    public string link;
    public string icon;
    public string smallDescription;
    public string smallDescriptionHTML;
    public Action[] actions;
  }

  [Serializable]
  public class FeedElements
  {
    public FeedElement[] items;
  }
}
