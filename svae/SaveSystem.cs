using Godot;
using System;

public static class SaveSystem
{
    private static string SavePath => "user://savegame.json";

    /// <summary>
    /// 保存数据到 JSON 文件
    /// </summary>
    public static void Save(Godot.Collections.Dictionary data)
    {
        var json = Json.Stringify(data, "\t"); // 格式化输出
        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        file.StoreLine(json);
        GD.Print("保存成功: " + SavePath);
    }

    /// <summary>
    /// 从 JSON 文件读取数据
    /// </summary>
    public static Godot.Collections.Dictionary Load()
    {
        if (!FileAccess.FileExists(SavePath))
        {
            GD.Print("存档不存在");
            return new Godot.Collections.Dictionary();
        }

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        var text = file.GetAsText();

        var result = Json.ParseString(text);
        if (result.VariantType == Variant.Type.Dictionary)
            return result.AsGodotDictionary();

        GD.PrintErr("存档格式错误");
        return new Godot.Collections.Dictionary();
    }

    /// <summary>
    /// 删除存档
    /// </summary>
    public static void Delete()
    {
        if (FileAccess.FileExists(SavePath))
        {
            DirAccess.RemoveAbsolute(SavePath);
            GD.Print("存档已删除");
        }
    }
}
