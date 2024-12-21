using UnityEngine;

public static class SaveSystem
{
    public static void ResetSaveData()
    {
        PlayerStats.ResetSavedData();
        SetBossClear(BossEnemyGrass.BOSS_ID, clear: false);
        // TODO water kingdom
        // TODO fire kingdom
        HasSave = false;
    }

    public static bool ClearedAllBosses()
    {
        var clearedAllBosses = true;
        clearedAllBosses &= GetBossClear(BossEnemyGrass.BOSS_ID);
        // TODO water kingdom
        // TODO fire kingdom
        return clearedAllBosses;
    }

    public static bool HasSave
    {
        get => PlayerPrefs.GetInt("HasSave", 0) != 0;
        set => PlayerPrefs.SetInt("HasSave", value ? 1 : 0);
    }

    public static bool GetBossClear(string bossId) => PlayerPrefs.GetInt($"Boss_{bossId}", 0) != 0;

    public static void SetBossClear(string bossId, bool clear = true)
    {
        if (clear)
        {
            PlayerPrefs.SetInt($"Boss_{bossId}", 1);
            HasSave = true;
        }
        else PlayerPrefs.DeleteKey($"Boss_{bossId}");
    }
}