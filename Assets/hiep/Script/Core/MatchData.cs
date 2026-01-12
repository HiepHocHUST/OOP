[System.Serializable]
public class MatchData
{
    // Các biến này khớp với cột trong bảng MatchHistory của bạn
    public int StageID;
    public int IsWin;        // 1 = Thắng, 0 = Thua
    public int GoldEarned;
    public string PlayDate;

    // Constructor để nạp nhanh dữ liệu
    public MatchData(int stage, int result, int gold, string date)
    {
        StageID = stage;
        IsWin = result;
        GoldEarned = gold;
        PlayDate = date;
    }
}