using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System; // Cần cái này để lấy DateTime, nhưng nó gây xung đột Random

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MaintenanceManager : MonoBehaviour
{
    [Header("--- 1. CẤU HÌNH CỘT TRÁI (DANH SÁCH SAO LƯU) ---")]
    public Transform contentHolder;
    public GameObject backupItemPrefab;

    [Header("--- 2. CẤU HÌNH CỘT PHẢI (KHÔI PHỤC) ---")]
    public InputField inputFilePath;
    public Button btnRestore;
    public Text textStatus;

    private List<string> fakeDates = new List<string>() { "2024-05-10", "2024-05-11", "2024-05-12" };

    void Start()
    {
        LoadFakeData();
    }

    void LoadFakeData()
    {
        foreach (Transform child in contentHolder) Destroy(child.gameObject);

        foreach (string date in fakeDates)
        {
            // --- SỬA LỖI Ở ĐÂY: Thêm chữ "UnityEngine." vào trước Random ---
            string randomSize = UnityEngine.Random.Range(10, 50) + " MiB";
            CreateBackupRow(date, randomSize);
        }
    }

    public void OnClick_CreateNew()
    {
        // Lấy ngày giờ hiện tại
        string timeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        CreateBackupRow(timeNow, "15.5 MiB");

        UpdateStatus("Đã tạo bản sao lưu mới: " + timeNow, Color.green);
    }

    void CreateBackupRow(string dateInfo, string sizeInfo)
    {
        if (backupItemPrefab == null || contentHolder == null)
        {
            Debug.LogError("LỖI: Chưa gắn Row Prefab hoặc Content vào Script!");
            return;
        }

        GameObject newRow = Instantiate(backupItemPrefab, contentHolder);
        newRow.transform.localScale = Vector3.one; // Ép về kích thước chuẩn (1,1,1)
        newRow.transform.localPosition = Vector3.zero; // Đưa về vị trí gốc để Layout Group tự xếp
        Text txtInfo = newRow.GetComponentInChildren<Text>();
        if (txtInfo != null)
        {
            txtInfo.text = "Sao lưu: " + dateInfo + " | " + sizeInfo;
        }

        Button btnDownload = newRow.GetComponentInChildren<Button>();
        if (btnDownload != null)
        {
            btnDownload.onClick.RemoveAllListeners();
            btnDownload.onClick.AddListener(() => OnClick_Download(dateInfo));
        }
    }

    // --- CÁC CHỨC NĂNG KHÁC ---

    public void OnClick_Download(string fileName)
    {
        Debug.Log("Đang tải file: " + fileName);

#if UNITY_EDITOR
        string safeName = fileName.Replace(":", "-").Replace(" ", "_");
        string path = EditorUtility.SaveFilePanel("Lưu file sao lưu", "", "Backup_" + safeName, "dat");

        if (!string.IsNullOrEmpty(path))
        {
            string content = "DỮ LIỆU GAME\nNgày: " + fileName + "\nLevel: 55\nVàng: 9999";
            File.WriteAllText(path, content);
            
            UpdateStatus("Đã tải xuống thành công!", Color.green);
            EditorUtility.RevealInFinder(path);
        }
#else
        UpdateStatus("Chức năng tải file chỉ hỗ trợ trong Unity Editor", Color.yellow);
#endif
    }

    public void OnClick_Browse()
    {
#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel("Chọn file phục hồi", "", "dat,txt");
        if (!string.IsNullOrEmpty(path))
        {
            inputFilePath.text = path;
        }
#endif
    }

    public void OnClick_Restore()
    {
        if (string.IsNullOrEmpty(inputFilePath.text))
        {
            UpdateStatus("Lỗi: Vui lòng chọn file trước!", Color.red);
            return;
        }

        if (btnRestore != null) btnRestore.interactable = false;

        UpdateStatus("Đang khôi phục dữ liệu...", Color.yellow);
        Invoke("RestoreCompleted", 2.0f);
    }

    void RestoreCompleted()
    {
        UpdateStatus("Khôi phục dữ liệu thành công!", Color.green);
        if (btnRestore != null) btnRestore.interactable = true;
    }

    void UpdateStatus(string message, Color color)
    {
        if (textStatus != null)
        {
            textStatus.text = message;
            textStatus.color = color;
        }
        Debug.Log(message);
    }
}