using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 특정 테이블의 모든 Row 객체를 ID를 키로 저장하고 조회하는 컨테이너 클래스입니다.
/// </summary>
/// <typeparam name="TKey">테이블의 ID(주 키) 타입</typeparam>
/// <typeparam name="TRow">TableBase를 상속받는 실제 Row 데이터 타입</typeparam>
public class Table<TKey, TRow> where TRow : TableBase
{
    private readonly Dictionary<TKey, TRow> _data;
    private readonly string _tableName;
    private readonly string _idColumnName;

    /// <summary>
    /// 테이블의 모든 데이터를 Dictionary 형태로 반환합니다.
    /// </summary>
    public IReadOnlyDictionary<TKey, TRow> Data => _data;

    public Table(string tableName, string idColumnName, Dictionary<TKey, TRow> data)
    {
        _tableName = tableName;
        _idColumnName = idColumnName;
        _data = data;
    }

    /// <summary>
    /// ID를 사용하여 Row 객체를 가져옵니다.
    /// </summary>
    /// <param name="id">조회할 레코드의 ID 값</param>
    /// <returns>해당 ID의 TRow 객체 또는 ID가 없는 경우 null</returns>
    public TRow this[TKey id]
    {
        get
        {
            if (_data.TryGetValue(id, out TRow row))
            {
                return row;
            }
            
            Debug.LogError($"[Table ERROR] 테이블 '{_tableName}'에 존재하지 않는 ID: '{id}' ({_idColumnName})");
            return null;
        }
    }
}