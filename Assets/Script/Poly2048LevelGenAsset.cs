// Poly2048LevelGenAsset.cs
#if UNITY_EDITOR
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Poly2048LevelGen", menuName = "Poly2048/Level Generator (Odin)")]
public class Poly2048LevelGenAsset : SerializedScriptableObject
{
    [Title("Board")]
    [MinValue(2)] public int rows = 6;
    [MinValue(2)] public int cols = 6;

    [Title("Target & Backward Steps")]
    [MinValue(2)] public int targetValue = 64;  // nên là 2^n
    [MinValue(1)] public int steps = 4;
    public int seed = 12345;

    [Title("Base Shape & Scale (grid units)")]
    public BaseShape shape = BaseShape.O_2x2;
    [MinValue(1)] public int sx = 1;
    [MinValue(1)] public int sy = 1;

    [ShowInInspector, ReadOnly, LabelText("Suggested baseMask preview")]
    private string maskPreview => string.Join(", ", GetBaseMask(shape));

    // ===== Odin BUTTONS =====

    [Button(ButtonSizes.Large), GUIColor(0.2f, 0.7f, 1f)]
    public void GenerateAndSaveJson()
    {
        try
        {
            var baseMask = GetBaseMask(shape);
            var level = SafeLevelGenerator.GenerateSafeLevel(
                rows, cols, baseMask, sx, sy, targetValue, steps, seed
            );

            string pretty = JsonUtility.ToJson(level, true);
            var defaultName = $"level_{rows}x{cols}_t{targetValue}_s{steps}_seed{seed}.json";
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Level JSON",
                defaultName,
                "json",
                "Chọn nơi lưu file JSON (nên để trong Assets/StreamingAssets/levels)"
            );
            if (string.IsNullOrEmpty(path)) return;

            // ensure folder
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            File.WriteAllText(path, pretty);
            AssetDatabase.Refresh();
            Debug.Log($"[Poly2048] Saved level JSON:\n{path}");

        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Poly2048] Generate failed: {e.Message}\n{e.StackTrace}");
            EditorUtility.DisplayDialog("Generate Failed", e.Message, "OK");
        }
    }

    [Button("Quick Preview (Console)")]
    public void PreviewToConsole()
    {
        var baseMask = GetBaseMask(shape);
        var level = SafeLevelGenerator.GenerateSafeLevel(
            rows, cols, baseMask, sx, sy, targetValue, steps, seed
        );
        Debug.Log($"Tiles: {level.tiles.Count}, Solution: {string.Join(" -> ", level.solutionMoves)}");
        Debug.Log(JsonUtility.ToJson(level, true));
    }

    // ===== Shapes =====
    public enum BaseShape { O_2x2, I_1x2, I_1x3, L_2x2 }

    public static Vector2Int[] GetBaseMask(BaseShape s)
    {
        switch (s)
        {
            case BaseShape.O_2x2:
                return new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) };
            case BaseShape.I_1x2:
                return new[] { new Vector2Int(0, 0), new Vector2Int(0, 1) }; // dọc
            case BaseShape.I_1x3:
                return new[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) }; // dọc
            case BaseShape.L_2x2:
                return new[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) }; // thiếu góc (0,1)
            default:
                return new[] { new Vector2Int(0, 0) };
        }
    }
}
#endif
