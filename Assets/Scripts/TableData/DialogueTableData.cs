public class DialogueTableData : TableBase
{
    public int dialogue_id { get; set; }
    public int char_id { get; set; }
    public int slot { get; set; }
    public int slot_state { get; set; }
    public string bg_res { get; set; }
    public string dialogue_str { get; set; }
    public int next_id { get; set; }
}
