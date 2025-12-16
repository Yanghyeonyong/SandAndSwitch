using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class TableClassGenerator
{
    // CSV 헤더 행 인덱스 설정
    private const int ColumnInternalNameRow = 1; // 컬럼명 (내부 변수 이름)
    private const int DataTypeRow = 2;           // 자료형 (int, float, bool, string)

    /// <summary>
    /// 지정된 폴더 내의 모든 TextAsset(CSV) 파일을 읽어 클래스 코드를 생성하고 저장합니다.
    /// </summary>
    /// <param name="csvFolderPath">CSV 파일이 위치한 Assets 내 폴더 경로</param>
    /// <param name="saveFolderPath">생성된 클래스 파일을 저장할 Assets 내 폴더 경로</param>
    public static void GenerateAllClassesInFolder(string csvFolderPath, string saveFolderPath)
    {
        // 1. 폴더 내 모든 TextAsset 파일의 Asset Path 목록을 가져옵니다.
        string[] assetGuids = AssetDatabase.FindAssets($"t:TextAsset", new[] { csvFolderPath });

        if (assetGuids.Length == 0)
        {
            Debug.LogWarning($"[Generator WARNING] 경로 '{csvFolderPath}'에서 TextAsset 파일을 찾을 수 없습니다.");
            EditorUtility.DisplayDialog("클래스 생성 완료",
                                        $"경로 '{csvFolderPath}'에서 CSV 파일을 찾지 못했습니다.",
                                        "확인");
            return;
        }

        int successCount = 0;

        // 2. 각 TextAsset 파일에 대해 파싱 및 생성 반복
        foreach (string guid in assetGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            TextAsset csvFile = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);

            if (csvFile == null) continue;

            Debug.Log($"[Generator] CSV 파일 처리 시작: {csvFile.name}");

            CsvClassData data = ParseCsvHeaders(csvFile);
            if (data == null) continue;

            string className = $"{data.TableName}Data";
            string code = GenerateClassCode(data);

            SaveClassFile(code, className, saveFolderPath, false); // 일괄 처리 시 대화 상자 비활성화
            successCount++;
        }

        // 3. 최종 결과 알림
        AssetDatabase.Refresh(); // 최종적으로 한번만 새로고침
        Debug.Log($"[Generator SUCCESS] 총 {successCount}개의 클래스 파일 생성 완료.");
        EditorUtility.DisplayDialog("클래스 생성 완료",
                                    $"총 {successCount}개의 테이블 클래스 파일이 다음 경로에 생성되었습니다:\n{saveFolderPath}",
                                    "확인");
    }

    /// <summary>
    /// CSV 파일을 읽어 클래스 생성에 필요한 데이터 구조를 반환합니다.
    /// </summary>
    public static CsvClassData ParseCsvHeaders(TextAsset csvFile)
    {
        if (csvFile == null) return null;

        var classData = new CsvClassData
        {
            TableName = Path.GetFileNameWithoutExtension(csvFile.name)
        };

        // 1. CSV 내용을 줄 단위로 분리
        string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= DataTypeRow)
        {
            Debug.LogError($"[Generator ERROR] CSV 파일 '{csvFile.name}'의 헤더 행이 부족합니다.");
            return null;
        }

        // 2. 컬럼명(내부명)과 자료형 행 추출
        string[] internalNames = lines[ColumnInternalNameRow].Split(',').Select(s => s.Trim()).ToArray();
        string[] dataTypes = lines[DataTypeRow].Split(',').Select(s => s.Trim()).ToArray();

        int columnCount = internalNames.Length;
        if (columnCount != dataTypes.Length)
        {
            Debug.LogError($"[Generator ERROR] 컬럼명({columnCount})과 자료형({dataTypes.Length})의 수가 일치하지 않습니다.");
            return null;
        }

        // 3. 컬럼 정의 리스트 생성
        for (int i = 0; i < columnCount; i++)
        {
            string name = internalNames[i];
            string type = dataTypes[i];

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
            {
                // 이름이나 타입이 비어있는 컬럼은 무시
                continue;
            }

            classData.Columns.Add(new ColumnDefinition { Name = name, Type = ConvertCSharpType(type) });
        }

        return classData;
    }

    /// <summary>
    /// CSV 자료형 문자열을 C# 타입 문자열로 변환합니다.
    /// </summary>
    private static string ConvertCSharpType(string csvType)
    {
        // 소문자로 변환하여 비교
        switch (csvType.ToLower())
        {
            case "int":
            case "enum":
                return "int";
            case "float":
                return "float";
            case "bool":
                return "bool";
            case "string":
            default:
                return "string";
        }
    }

    /// <summary>
    /// CsvClassData를 기반으로 C# 클래스 코드를 생성합니다.
    /// </summary>
    public static string GenerateClassCode(CsvClassData data)
    {
        string className = $"{data.TableName}Data";
        var sb = new StringBuilder();

        sb.AppendLine($"public class {className} : TableBase");
        sb.AppendLine("{");

        // 컬럼 속성 추가
        foreach (var col in data.Columns)
        {
            // public auto property 사용 (TableParser에서 리플렉션으로 값을 채우기 위함)
            sb.AppendLine($"    public {col.Type} {col.Name} {{ get; set; }}");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <summary>
    /// 생성된 C# 코드를 파일로 저장합니다.
    /// </summary>
    /// <param name="code">저장할 C# 클래스 코드</param>
    /// <param name="className">생성될 클래스 이름</param>
    /// <param name="savePath">저장할 폴더 경로</param>
    /// <param name="dialogEnabled">저장 성공/실패 대화 상자를 표시할지 여부</param>
    public static void SaveClassFile(string code, string className, string savePath, bool dialogEnabled = true)
    {
        // 파일 경로 설정
        string fullPath = Path.Combine(savePath, $"{className}.cs");

        try
        {
            // 폴더가 없으면 생성
            Directory.CreateDirectory(savePath);
            File.WriteAllText(fullPath, code, Encoding.UTF8);

            if (dialogEnabled)
            {
                // Unity 에디터에 변경 사항을 알림
                AssetDatabase.Refresh();
                Debug.Log($"[Generator SUCCESS] 클래스 파일 생성 완료: {fullPath}");
                EditorUtility.DisplayDialog("클래스 생성 성공",
                                            $"'{className}.cs' 파일이 다음 경로에 생성되었습니다:\n{savePath}",
                                            "확인");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Generator ERROR] 파일 저장 실패: {e.Message}");
            if (dialogEnabled)
            {
                EditorUtility.DisplayDialog("클래스 생성 실패",
                                            $"파일 저장 중 오류가 발생했습니다:\n{e.Message}",
                                            "확인");
            }
        }
    }
}