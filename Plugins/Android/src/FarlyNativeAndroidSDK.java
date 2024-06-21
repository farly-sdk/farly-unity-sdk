package com.farly.nativeandroidunity;

import android.app.Activity;
import android.util.Log;
import androidx.annotation.Nullable;
import androidx.annotation.NonNull;

import com.farly.farly.Farly;
import com.farly.farly.Farly.OfferWallPresentationMode;
import com.farly.farly.jsonmodel.FeedItem;
import com.farly.farly.model.OfferWallRequest;
import com.unity3d.player.UnityPlayer;
import com.google.gson.Gson;

import java.util.List;
import java.util.Date;

interface FarlyNativeAndroidSDKPluginInterface {
    void configure(String publisherId);

    void getHostedOfferwallUrl(String requestJSON);

    void showOfferwallInWebview(String requestJSON);

    void showOfferwallInBrowser(String requestJSON);

    void getOfferwall(String requestJSON);
}

class SerializableOfferwallRequest extends OfferWallRequest {
    @Nullable
    private String userSignupDateTimestamp;

    public SerializableOfferwallRequest(@NonNull String userId) {
        super(userId);
    }

    public OfferWallRequest toOfferwallRequest() {
        Date userSignupDate = null;
        if (userSignupDateTimestamp != null && userSignupDateTimestamp.trim().length() > 0) {
            userSignupDate = new Date(Long.parseLong(userSignupDateTimestamp) * 1000);
        }
        this.setUserSignupDate(userSignupDate);
        return this;
    }

    public String getUserSignupDateTimestamp() {
        return userSignupDateTimestamp;
    }

    public void setUserSignupDateTimestamp(@Nullable String userSignupDateTimestamp) {
        this.userSignupDateTimestamp = userSignupDateTimestamp;
    }
}

public class FarlyNativeAndroidSDK implements FarlyNativeAndroidSDKPluginInterface {
    private static final String TAG = "FarlyNativeAndroidSDK";
    private static final String GAME_OBJECT_NAME = "FarlyPluginBridge";
    private static Gson gson = new Gson();
    private Activity mainActivity;

    public FarlyNativeAndroidSDK(Activity activity) {
        this.mainActivity = activity;
        Log.d(TAG, "Initialized FarlyNativeAndroidSDKPlugin class");
    }

    private OfferWallRequest parseRequest(String requestJSON) {
        SerializableOfferwallRequest request = gson.fromJson(requestJSON, SerializableOfferwallRequest.class);
        return request.toOfferwallRequest();
    }

    @Override
    public void configure(String publisherId) {
        Farly.getInstance().setPublisherId(publisherId);
    }

    @Override
    public void getHostedOfferwallUrl(String requestJSON) {
        try {
            OfferWallRequest request = parseRequest(requestJSON);
            String url = Farly.getInstance().getHostedOfferWallUrl(this.mainActivity, request);
            UnityPlayer.UnitySendMessage(GAME_OBJECT_NAME, "HandleAsyncOfferwallUrlAnswer", url);
        } catch (Exception exception) {
            UnityPlayer.UnitySendMessage(GAME_OBJECT_NAME, "HandleException", exception.toString());
        }
    }

    @Override
    public void showOfferwallInWebview(String requestJSON) {
        try {
            OfferWallRequest request = parseRequest(requestJSON);
            Farly.getInstance().showOfferWall(this.mainActivity, request, OfferWallPresentationMode.WEB_VIEW);
        } catch (Exception exception) {
            UnityPlayer.UnitySendMessage(GAME_OBJECT_NAME, "HandleException", exception.toString());
        }
    }

    @Override
    public void showOfferwallInBrowser(String requestJSON) {
        try {
            OfferWallRequest request = parseRequest(requestJSON);
            Farly.getInstance().showOfferWall(this.mainActivity, request, OfferWallPresentationMode.BROWSER);
        } catch (Exception exception) {
            UnityPlayer.UnitySendMessage(GAME_OBJECT_NAME, "HandleException", exception.toString());
        }
    }

    @Override
    public void getOfferwall(String requestJSON) {
        try {
            OfferWallRequest request = parseRequest(requestJSON);
            Farly.getInstance().getOfferWall(this.mainActivity, request, new Farly.OfferWallRequestCompletionHandler() {
                @Override
                public void onComplete(List<FeedItem> feed) {
                    UnityPlayer.UnitySendMessage(GAME_OBJECT_NAME, "HandleAsyncOfferwallAnswer", gson.toJson(feed));
                }

                @Override
                public void onError(Exception e) {
                    UnityPlayer.UnitySendMessage(GAME_OBJECT_NAME, "HandleException", e.toString());
                }
            });
        } catch (Exception exception) {
            UnityPlayer.UnitySendMessage(GAME_OBJECT_NAME, "HandleException", exception.toString());
        }
    }
}
