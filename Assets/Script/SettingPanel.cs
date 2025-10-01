using UnityEngine;
using UnityEngine.UI;


public class SettingPanel : MonoBehaviour
{
    public Slider sliderMusic;
    public Slider sliderSfx;
    public Slider sliderVib;
    public Button btnClose;
    public Button btnRetry;

    const string KEY_MUSIC = "vol_music";
    const string KEY_SFX = "vol_sfx";
    const string KEY_VIBE = "vibrate";
    public void Init()
    {
        btnClose.onClick.AddListener(()=> this.gameObject.SetActive(false));
        //btnRetry.onClick.AddListener(() => GamePlayController.Instance.playerContaint.HandleRetry());
        float music = PlayerPrefs.GetFloat(KEY_MUSIC, 0.8f);
        float sfx = PlayerPrefs.GetFloat(KEY_SFX, 0.8f);
        float vibe = PlayerPrefs.GetFloat(KEY_VIBE, 1f);
        sliderMusic.SetValueWithoutNotify(music);
        sliderSfx.SetValueWithoutNotify(sfx);
        sliderVib.SetValueWithoutNotify(vibe);

        ApplyMusic(music);
        ApplySfx(sfx);

        sliderMusic.onValueChanged.AddListener(v => {
            ApplyMusic(v);
            PlayerPrefs.SetFloat(KEY_MUSIC, v);
        });

        sliderSfx.onValueChanged.AddListener(v => {
            ApplySfx(v);
            PlayerPrefs.SetFloat(KEY_SFX, v);
        });

        sliderVib.onValueChanged.AddListener(v => {
            PlayerPrefs.SetFloat(KEY_VIBE, v);
        });
        //GameManager.Instance.musicController.HandleMusic();
    }
    void ApplyMusic(float v)
    {
        //GameManager.Instance.musicController.audioMusic.volume = v;
    }

    void ApplySfx(float v)
    {
        //GameManager.Instance.musicController.audioSouceSfx.volume = v;
    }

    public void TryVibrate()
    {
        // Đảm bảo đã bật trong setting
        float vibeSetting = PlayerPrefs.GetFloat(KEY_VIBE, 1f);
        Debug.Log($"TryVibrate được gọi - Vibe setting: {vibeSetting}");
        
        if (vibeSetting > 0.5f)
        {
            Debug.Log("Vibration được bật, gọi VibrationMng.Vibrate()");
            //VibrationMng.Vibrate();
        }
        else
        {
            Debug.Log("Vibration bị tắt trong settings");
        }
    }

}
