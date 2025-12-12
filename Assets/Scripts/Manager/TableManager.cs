using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 모든 테이블 데이터를 로드하고 정적으로 관리하는 중앙 관리자입니다.
/// </summary>
public class TableManager : Singleton<TableManager>
{
    // ID 컬럼명에 대한 기본값
    [SerializeField] private const string DefaultIdColumnName = "id";

    [Header("CSV 파일 경로(Resources 폴더 내의 경로)")]
    [SerializeField] private string _csvPath = "Data";

    // 로드할 테이블 Row 클래스 정보 (TRow Type, TKey Type, ID 컬럼명) 매핑
    private static readonly Dictionary<Type, string> TableMap = new Dictionary<Type, string>()
    {
        // { TRow Type, ID 컬럼명 (덮어쓸 값) }
        // ItemTableData의 ID 컬럼명이 "id"가 아니라 "itemid"이므로 명시적으로 등록
        { typeof(CharacterTableData), "char_id" }, 
        { typeof(DialogueTableData), "dialogue_id" },
        { typeof(StringTableData), "key_index" }
        // { typeof(ToppingData), "toppingid" }, //ex) "id"가 아닌 다른 컬럼명을 쓰는 경우만 등록해주면 됩니다.
    };

    // 정적(Static) 테이블 컨테이너 저장소 (Type: TRow, Value: TableBase)
    private static readonly Dictionary<Type, object> LoadedTables = new Dictionary<Type, object>();

    // 테이블 클래스 이름을 TRow Type에 매핑, 실제 CSV파일명과 클래스명을 매칭시켜 사용하기 위함
    private static Dictionary<string, Type> TableNameToTypeMap;


    protected override void Awake()
    {
        base.Awake();

        // 런타임에 모든 TableBase 상속 클래스를 한 번만 찾아서 Map을 구축합니다.
        BuildTableNameToTypeMap();

        LoadAllTables();
    }

    /// <summary>
    /// 현재 어셈블리에서 TableBase를 상속받는 모든 클래스를 찾아 맵핑을 구축합니다.
    /// </summary>
    private static void BuildTableNameToTypeMap()
    {
        TableNameToTypeMap = new Dictionary<string, Type>();

        // 현재 실행 중인 어셈블리들을 모두 가져옵니다.
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        Type tableBaseType = typeof(TableBase);

        foreach (var assembly in assemblies)
        {
            // TableBase를 상속받는 모든 public 클래스를 찾습니다.
            var rowTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(tableBaseType));

            foreach (var rowType in rowTypes)
            {
                // CSV 파일명과 일치할 것으로 예상되는 이름 (예: "ItemTableData" -> "ItemTable")
                string tableName = rowType.Name.Replace("Data", "");
                TableNameToTypeMap[tableName] = rowType;
            }
        }

        Debug.Log($"[TableManager] {TableNameToTypeMap.Count}개의 TRow 클래스 매핑 완료.");
    }


    private void LoadAllTables()
    {
        Debug.Log("[TableManager] 모든 테이블 로드를 시작합니다...");

        // Resources 폴더 내의 _csvPath 경로에서 모든 TextAsset 파일을 로드합니다.
        TextAsset[] csvFiles = Resources.LoadAll<TextAsset>($"{_csvPath}");

        if (csvFiles.Length == 0)
        {
            Debug.LogError($"[TableManager ERROR] 'Resources/{_csvPath}' 경로에서 CSV 파일을 찾지 못했습니다. 파일 위치를 확인하세요.");
            return;
        }

        // CSV 파일을 순회하며 로드를 시도합니다. (csvFiles 기준으로 순회)
        foreach (var csvFile in csvFiles)
        {
            // 1. CSV 파일명으로부터 예상되는 TRow 클래스 이름(tableName)을 추출
            string csvFileName = csvFile.name; // ex) "ItemTable"

            // 2. TRow 클래스 타입을 찾아옵니다.
            if (!TableNameToTypeMap.TryGetValue(csvFileName, out Type rowType))
            {
                Debug.LogWarning($"[TableManager WARNING] CSV 파일 '{csvFileName}'에 대응하는 TRow 클래스 ({csvFileName}Data)가 존재하지 않아 로드를 건너뜁니다.");
                continue;
            }

            // 3. ID 컬럼명 결정: TableMap에 명시적으로 등록된 ID가 있는지 확인합니다.
            string idColumnName = DefaultIdColumnName;
            if (TableMap.TryGetValue(rowType, out string mappedIdName))
            {
                idColumnName = mappedIdName; // TableMap에 등록된 값으로 덮어씁니다.
            }

            // 3-1. ID 컬럼명을 가진 멤버의 타입을 찾아 ID 타입을 결정합니다.
            MemberInfo idMember = rowType.GetMember(idColumnName, BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();

            if (idMember == null)
            {
                Debug.LogError($"[TableManager ERROR] '{rowType.Name}'에서 ID 컬럼 '{idColumnName}'에 해당하는 필드를 찾지 못했습니다.");
                continue;
            }

            Type keyType = null;

            if (idMember is PropertyInfo pInfo)
            {
                keyType = pInfo.PropertyType;
            }
            else if (idMember is FieldInfo fInfo)
            {
                keyType = fInfo.FieldType;
            }

            if (keyType == null)
            {
                Debug.LogError($"[TableManager ERROR] '{rowType.Name}'의 ID 타입을 확인할 수 없습니다.");
                continue;
            }

            Debug.Log($"[TableManager] '{rowType.Name}' ID 타입 감지됨 → {keyType.Name}");

            // 4. 테이블 로드


            try
            {
                // TableParser의 Parse 메서드 정보를 가져와 제네릭 버전을 만듭니다.
                Type tableParserType = typeof(TableParser);
                MethodInfo parseMethod = tableParserType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static);

                if (parseMethod == null)
                {
                    Debug.LogError("[TableManager ERROR] TableParser.Parse<TKey, TRow> 메서드를 찾을 수 없습니다.");
                    continue;
                }

                MethodInfo genericParseMethod = parseMethod.MakeGenericMethod(keyType, rowType);

                // Parse 메서드 실행
                object loadedTable = genericParseMethod.Invoke(null, new object[] { csvFile, idColumnName });

                if (loadedTable != null)
                {
                    LoadedTables[rowType] = loadedTable;
                    Debug.Log($"[TableManager SUCCESS] '{rowType.Name}' 테이블 로드 완료. (ID 컬럼: '{idColumnName}')");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[TableManager ERROR] 테이블 '{rowType.Name}' 로드 중 오류 발생 (ID 컬럼: '{idColumnName}'): {e.InnerException?.Message ?? e.Message}");
            }
        }

        Debug.Log($"[TableManager] 총 {LoadedTables.Count}개 테이블 로드 완료.");
    }


    /// <summary>
    /// 특정 타입의 Table 컨테이너에 저장된 모든 Key(ID)의 리스트를 반환합니다.
    /// </summary>
    /// <typeparam name="TKey">테이블의 ID(주 키) 타입</typeparam>
    /// <typeparam name="TRow">TableBase를 상속받는 실제 Row 데이터 타입</typeparam>
    /// <param name="table">ID 리스트를 가져올 Table 컨테이너</param>
    /// <returns>모든 ID를 포함하는 List<TKey> 또는 테이블이 null이면 빈 리스트</returns>
    public List<TKey> GetAllIds<TKey, TRow>(Table<TKey, TRow> table) where TRow : TableBase
    {
        if (table == null)
        {
            // Error 로그는 TableManager.cs의 인덱서에서 이미 출력되므로 생략 가능
            return new List<TKey>();
        }

        return table.Data.Keys.ToList();
    }

    /// <summary>
    /// 로드된 테이블 저장소에서 특정 TRow 타입에 해당하는 Table 객체를 가져옵니다.
    /// </summary>
    /// <typeparam name="TKey">테이블의 ID 타입</typeparam>
    /// <typeparam name="TRow">테이블의 Row 데이터 타입</typeparam>
    public Table<TKey, TRow> GetTable<TKey, TRow>() where TRow : TableBase
    {
        if (LoadedTables.TryGetValue(typeof(TRow), out object tableObject))
        {
            // object를 Table<TKey, TRow> 타입으로 캐스팅하여 반환
            return tableObject as Table<TKey, TRow>;
        }
        return null;
    }
}