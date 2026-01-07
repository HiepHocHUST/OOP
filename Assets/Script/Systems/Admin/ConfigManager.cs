using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ConfigManager : MonoBehaviour
{
    [Header("--- KẾT NỐI UI ---")]
    public InputField inputAndroid;
    public InputField inputIOS;
    public Dropdown dropdownResolution;
    public Slider sliderVolume;

    // Đã xóa biến TextVolumePercent ở đây rồi nhé!

    [Header("--- KẾT NỐI HỆ THỐNG ---")]
    public AdminManager adminManager;

    private Resolution[] resolutions;

    void Start()
    {
        InitResolutionDropdown();
        LoadSettings();
        sliderVolume.onValueChanged.AddListener(OnVolumeChanged);
    }

    // --- CÁC HÀM XỬ LÝ ---

    public void OnVolumeChanged(float value)
    {
        // Chỉ xử lý âm thanh, không cần cập nhật Text nữa
        // (Bạn có thể thêm code chỉnh âm thanh thật ở đây sau này)
    }

    public void OnClick_BrowseAndroid()
    {
        string path = OpenFileBrowser("Chọn file APK", "apk");
        if (!string.IsNullOrEmpty(path)) inputAndroid.text = path;
    }

    public void OnClick_BrowseIOS()
    {
        string path = OpenFileBrowser("Chọn file IPA", "ipa");
        if (!string.IsNullOrEmpty(path)) inputIOS.text = path;
    }

    string OpenFileBrowser(string title, string extension)
    {
#if UNITY_EDITOR
        return EditorUtility.OpenFilePanel(title, "", extension);
#else
        return "";
#endif
    }

    void InitResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        dropdownResolution.ClearOptions();
        List<string> options = new List<string>();
        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentResIndex = i;
        }
        dropdownResolution.AddOptions(options);
        dropdownResolution.value = currentResIndex;
    }

    public void OnClick_Save()
    {
        PlayerPrefs.SetString("Cfg_Android", inputAndroid.text);
        PlayerPrefs.SetString("Cfg_IOS", inputIOS.text);
        PlayerPrefs.SetInt("Cfg_Res", dropdownResolution.value);
        PlayerPrefs.SetFloat("Cfg_Vol", sliderVolume.value);

        Resolution res = resolutions[dropdownResolution.value];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.Save();
        Debug.Log("Đã Lưu!");
    }

    public void OnClick_Reset()
    {
        inputAndroid.text = "";
        inputIOS.text = "";
        sliderVolume.value = 1.0f;
        if (resolutions.Length > 0) dropdownResolution.value = resolutions.Length - 1;
    }

    public void OnClick_Close()
    {
        if (adminManager != null) adminManager.OnClick_BackToDashboard();
        else gameObject.SetActive(false);
    }

    void LoadSettings()
    {
        inputAndroid.text = PlayerPrefs.GetString("Cfg_Android", "");
        inputIOS.text = PlayerPrefs.GetString("Cfg_IOS", "");
        sliderVolume.value = PlayerPrefs.GetFloat("Cfg_Vol", 1.0f);
        dropdownResolution.value = PlayerPrefs.GetInt("Cfg_Res", resolutions.Length - 1);
    }
}