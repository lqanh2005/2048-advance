using UnityEngine;

public static class VibrationMng
{
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    private static AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    private static AndroidJavaObject vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
    private static AndroidJavaObject unityPlayer;
    private static AndroidJavaObject activity;
    private static AndroidJavaObject vibrator;
#endif

    public static void Vibrate(long milliseconds = 500)
    {
        Debug.Log($"Vibrate được gọi - Platform: {Application.platform}, IsAndroid: {IsAndroid()}");

        if (IsAndroid())
        {
            try
            {
                vibrator.Call("vibrate", milliseconds);
                Debug.Log($"Android vibration thành công - {milliseconds}ms");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Lỗi Android vibration: {e.Message}");
            }
        }
        else
        {
            try
            {
                Handheld.Vibrate();
                Debug.Log("Handheld.Vibrate() được gọi");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Lỗi Handheld vibration: {e.Message}");
            }
        }
    }

    public static void Cancel()
    {
        if (IsAndroid())
        {
            try
            {
                vibrator.Call("cancel");
                Debug.Log("Android vibration đã hủy");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Lỗi hủy Android vibration: {e.Message}");
            }
        }
    }

    public static bool IsAndroid()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }
}
