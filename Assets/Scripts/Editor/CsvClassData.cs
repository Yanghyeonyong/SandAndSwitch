using System.Collections.Generic;

/// <summary>
/// CSV 파일의 컬럼 정의(이름과 타입)를 저장하는 데이터 구조체
/// </summary>
public struct ColumnDefinition
{
    public string Name;
    public string Type;
}

/// <summary>
/// CSV 파일의 헤더 정보를 저장하는 컨테이너
/// </summary>
public class CsvClassData
{
    public string TableName { get; set; }
    public List<ColumnDefinition> Columns { get; set; } = new List<ColumnDefinition>();
}