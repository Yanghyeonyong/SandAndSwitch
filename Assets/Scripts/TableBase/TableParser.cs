using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// CSV 파일을 파싱하고 정의된 Row 클래스에 데이터를 자동으로 채워넣는 정적 파서 클래스입니다.
/// </summary>
public static class TableParser
{
    // CSV 헤더 행 인덱스 (0부터 시작, 2번째 줄)
    private const int HeaderRowIndex = 1;
    // 실제 데이터 시작 행 인덱스 (0부터 시작, 4번째 줄)
    private const int DataStartRowIndex = 3;

    /// <summary>
    /// CSV 파일을 파싱하여 제네릭 Table 컨테이너를 생성합니다.
    /// </summary>
    /// <typeparam name="TKey">ID 컬럼의 타입</typeparam>
    /// <typeparam name="TRow">TableBase를 상속받는 데이터 Row 클래스</typeparam>
    /// <param name="csvFile">유니티 TextAsset으로 로드된 CSV 파일</param>
    /// <param name="idColumnName">ID로 사용할 컬럼 이름 (CSV 헤더와 일치해야 함)</param>
    /// <returns>파싱된 데이터를 담은 Table 컨테이너</returns>
    public static Table<TKey, TRow> Parse<TKey, TRow>(
        TextAsset csvFile,
        string idColumnName)
        where TRow : TableBase, new()
    {
        if (csvFile == null)
        {
            Debug.LogError("[TableParser ERROR] CSV TextAsset이 null입니다.");
            return null;
        }

        // 1. CSV 내용 읽기 및 줄/셀 분리
        List<string[]> rows;
        try
        {
            rows = csvFile.text
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(',').Select(cell => cell.Trim()).ToArray())
                .ToList();
        }
        catch (Exception e)
        {
            Debug.LogError($"[TableParser ERROR] CSV 파일 읽기 오류: {e.Message}");
            return null;
        }

        // 2. 컬럼 헤더 추출 및 유효성 검사
        if (rows.Count <= HeaderRowIndex)
        {
            Debug.LogError($"[TableParser ERROR] CSV에 컬럼 헤더(인덱스 {HeaderRowIndex})가 없습니다.");
            return null;
        }
        string[] columnNames = rows[HeaderRowIndex];

        int idIndex = Array.IndexOf(columnNames, idColumnName);
        if (idIndex < 0)
        {
            Debug.LogError($"[TableParser ERROR] ID 컬럼 '{idColumnName}'이(가) CSV 헤더에 존재하지 않습니다.");
            return null;
        }

        // 3. TRow 클래스의 속성(Property) 및 필드(Field)와 컬럼명 매핑
        Type rowType = typeof(TRow);
        Dictionary<string, MemberInfo> memberMap = new Dictionary<string, MemberInfo>();
        foreach (var name in columnNames)
        {
            // 요구사항 1: Property 또는 Field를 찾아서 매핑
            var prop = rowType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                memberMap.Add(name, prop);
                continue;
            }

            var field = rowType.GetField(name, BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                memberMap.Add(name, field);
                continue;
            }
        }

        // 4. 데이터 파싱 및 TRow 인스턴스 생성
        Dictionary<TKey, TRow> allRowsData = new Dictionary<TKey, TRow>();

        for (int i = DataStartRowIndex; i < rows.Count; i++)
        {
            string[] rowValues = rows[i];

            if (rowValues.Length < columnNames.Length)
            {
                if (rowValues.All(string.IsNullOrEmpty)) continue;
                continue;
            }

            // ID 추출 및 TKey 타입으로 변환
            string idString = rowValues[idIndex];
            if (string.IsNullOrEmpty(idString)) continue;

            TKey idValue;
            try
            {
                idValue = (TKey)Convert.ChangeType(idString, typeof(TKey));
            }
            catch (Exception e)
            {
                Debug.LogError($"[TableParser ERROR] ID '{idString}' 변환 실패 (행 {i + 1}). {e.Message}");
                continue;
            }

            if (allRowsData.ContainsKey(idValue))
            {
                Debug.LogError($"[TableParser ERROR] 중복 ID 발견: '{idValue}'. (행 {i + 1})");
                continue;
            }

            TRow newRow = new TRow();

            // 인스턴스에 값 채우기
            for (int j = 0; j < columnNames.Length; j++)
            {
                string columnName = columnNames[j];
                string stringValue = rowValues[j];

                if (memberMap.TryGetValue(columnName, out MemberInfo member))
                {
                    Type targetType = (member is PropertyInfo prop) ? prop.PropertyType : ((FieldInfo)member).FieldType;

                    try
                    {
                        object convertedValue = ConvertValue(stringValue, targetType);

                        if (member is PropertyInfo propInfo)
                        {
                            propInfo.SetValue(newRow, convertedValue);
                        }
                        else if (member is FieldInfo fieldInfo)
                        {
                            fieldInfo.SetValue(newRow, convertedValue);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[TableParser ERROR] 테이블: {rowType.Name}, ID: {idValue}, 컬럼: {columnName}, 값: '{stringValue}' 변환 실패. {e.Message}");
                    }
                }
            }

            allRowsData.Add(idValue, newRow);
        }

        string tableName = csvFile.name;
        Debug.Log($"[TableParser SUCCESS] '{tableName}' 테이블 파싱 완료. {allRowsData.Count}개 레코드 로드.");
        return new Table<TKey, TRow>(tableName, idColumnName, allRowsData);
    }

    /// <summary>
    /// 문자열 값을 대상 C# 타입으로 변환합니다.
    /// </summary>
    private static object ConvertValue(string value, Type targetType)
    {
        // 공백 제거
        value = value.Trim();

        // Null 또는 빈 문자열 처리
        if (string.IsNullOrEmpty(value))
        {
            // Nullable이 아닌 값 타입(int, float 등)은 기본값(0) 반환
            if (targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null)
            {
                return Activator.CreateInstance(targetType);
            }
            return null; // string, Nullable 타입은 null 반환
        }

        // Bool 타입 처리 (CSV에서 '1', '0' 또는 'True', 'False' 처리)
        if (targetType == typeof(bool))
        {
            if (int.TryParse(value, out int intValue))
            {
                return intValue != 0;
            }
        }

        // Convert.ChangeType은 int, float, string 등 기본 타입 변환을 처리
        return Convert.ChangeType(value, targetType);
    }
}