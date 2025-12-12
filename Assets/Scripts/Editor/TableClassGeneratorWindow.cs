using UnityEditor;
using UnityEngine;
using System.IO;

public class TableClassGeneratorWindow : EditorWindow
{
    private string _targetFolder = "Assets/Resources/Data"; // CSV가 저장된 기본 경로
    private string _saveFolder = "Assets/Scripts/TableData"; // 클래스 파일 저장 기본 경로

    // 유니티 메뉴에 윈도우 추가
    [MenuItem("Tools/Table Parser/Table Class Generator")]
    public static void ShowWindow()
    {
        GetWindow<TableClassGeneratorWindow>("Table Class Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Set Table Path", EditorStyles.boldLabel);

        // --- 1. CSV 파일 폴더 설정 ---
        EditorGUILayout.Space();
        GUILayout.Label("1. CSV 파일 (TextAsset) 저장 경로", EditorStyles.miniLabel);

        EditorGUILayout.BeginHorizontal();
        _targetFolder = EditorGUILayout.TextField("CSV 폴더 경로", _targetFolder);
        if (GUILayout.Button("선택", GUILayout.Width(60)))
        {
            _targetFolder = SelectFolder(_targetFolder);
        }
        EditorGUILayout.EndHorizontal();

        // --- 2. 클래스 저장 경로 설정 ---
        EditorGUILayout.Space();
        GUILayout.Label("2. 클래스 파일 저장 경로", EditorStyles.miniLabel);

        EditorGUILayout.BeginHorizontal();
        _saveFolder = EditorGUILayout.TextField("저장 폴더 경로", _saveFolder);
        if (GUILayout.Button("선택", GUILayout.Width(60)))
        {
            _saveFolder = SelectFolder(_saveFolder);
        }
        EditorGUILayout.EndHorizontal();

        // --- 3. 클래스 생성 버튼 ---
        EditorGUILayout.Space(20);

        // 폴더가 유효하고 Assets 내부에 있을 때만 버튼 활성화
        bool canGenerate = Directory.Exists(_targetFolder) && _targetFolder.StartsWith("Assets");
        GUI.enabled = canGenerate;

        if (GUILayout.Button($"'{_targetFolder}' 폴더 내 모든 CSV 클래스 생성", GUILayout.Height(30)))
        {
            TableClassGenerator.GenerateAllClassesInFolder(_targetFolder, _saveFolder);
        }
        GUI.enabled = true;

        if (!canGenerate)
        {
            EditorGUILayout.HelpBox("CSV 폴더 경로가 유효하지 않거나 'Assets' 폴더 내에 있어야 합니다.", MessageType.Error);
        }
    }

    // 폴더 선택 로직 추출
    private string SelectFolder(string currentPath)
    {
        string selectedPath = EditorUtility.OpenFolderPanel("폴더 선택", currentPath, "");
        if (!string.IsNullOrEmpty(selectedPath))
        {
            // Unity Assets 경로로 변환
            if (selectedPath.StartsWith(Application.dataPath))
            {
                return "Assets" + selectedPath.Substring(Application.dataPath.Length);
            }
            EditorUtility.DisplayDialog("경로 오류", "Unity 프로젝트의 Assets 폴더 내 경로를 선택해야 합니다.", "확인");
        }
        return currentPath;
    }
}