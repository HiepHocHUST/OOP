using UnityEngine;
using UnityEngine.UI;

public class BattleRow : MonoBehaviour
{
    // 4 biến này để bạn kéo thả các Text UI vào
    public Text txtTime;
    public Text txtStage;
    public Text txtResult;
    public Text txtGold;

    // Hàm này sẽ được HistoryManager gọi để nạp dữ liệu
    public void SetupRow(string time, int stage, int result, int gold)
    {
        // 1. Điền thời gian
        txtTime.text = time;

        // 2. Điền tên màn chơi
        txtStage.text = "Màn " + stage;

        // 3. Điền kết quả và đổi màu chữ
        if (result == 1)
        {
            txtResult.text = "THẮNG";
            txtResult.color = Color.green; // Thắng màu xanh lá
        }
        else
        {
            txtResult.text = "THUA";
            txtResult.color = Color.red;   // Thua màu đỏ
        }

        // 4. Điền số vàng (định dạng có dấu phẩy, ví dụ: 50,000)
        txtGold.text = gold.ToString("N0");
    }
}