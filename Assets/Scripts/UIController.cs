using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Icons;

public class UIController : MonoBehaviour
{
    [Header("배경 이미지")]
    [SerializeField] private Image bgImage;

    [Header("캐릭터 슬롯 (좌 > 우)")]
    [SerializeField] private Image[] characterSlots = new Image[5];

    [Header("텍스트")]
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtDialogue;
    [SerializeField] private TMP_Text txtFaction;

    private Table<int, DialogueTableData> dlgTable;
    private Table<int, CharacterTableData> charTable;
    private Table<string, StringTableData> strTable;

    [SerializeField] private Language currentLanguage = Language.Ko;

    private int currentId;

    private void Start()
    {
        // 테이블 불러오기
        dlgTable = TableManager.Instance.GetTable<int, DialogueTableData>();
        charTable = TableManager.Instance.GetTable<int, CharacterTableData>();
        strTable = TableManager.Instance.GetTable<string, StringTableData>();

        // Dialogue 첫 행 자동 로드 (CSV 첫 데이터 ID 사용)
        currentId = dlgTable.Data.Keys.Min();
        LoadDialogue(currentId);
    }

    public void OnNextKeyDown()
    {
        // next_id가 비어 있으면 종료
        var row = dlgTable[currentId];
        if (row.next_id == 0)
        {
            Debug.Log("대사 종료");
            return;
        }

        currentId = row.next_id;
        LoadDialogue(currentId);
    }

    private void LoadDialogue(int id)
    {
        var dlg = dlgTable[id];
        var chr = charTable[dlg.char_id];

        // --- 배경 이미지 ---
        if (!string.IsNullOrEmpty(dlg.bg_res))
            bgImage.sprite = Resources.Load<Sprite>($"BG/{dlg.bg_res}");

        // --- 캐릭터 이미지 적용 ---
        ApplyCharacterToSlot(dlg.slot, chr.char_res);

        // --- slot_state 적용 ---
        ApplySlotStates(dlg.slot_state);


        // --- 이름 / 소속 ---
        txtName.text = GetLocalizedText(chr.name_str);
        txtFaction.text = GetLocalizedText(chr.faction_str);

        // --- 대사 ---
        txtDialogue.text = GetLocalizedText(dlg.dialogue_str);
    }

    private void ApplySlotStates(int slotState)
    {
        string data = slotState.ToString();

        if (data.Length < 6)
        {
            Debug.LogWarning($"slot_state 형식이 잘못됨: {data}");
            return;
        }

        // 첫 자리 제거 (무조건 1)
        data = data.Substring(1);   // 예: "21020"

        for (int i = 0; i < 5; i++)
        {
            int v = data[i] - '0';
            ApplySlotStateToImage(characterSlots[i], v);
        }
    }

    private void ApplySlotStateToImage(Image img, int state)
    {
        switch (state)
        {
            case 0:
                img.color = new Color(1, 1, 1, 0);
                break;

            case 1:
                img.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                break;

            case 2:
                img.color = Color.white;
                break;
        }
    }

    private void ApplyCharacterToSlot(int slot, string res)
    {
        if (slot <= 0 || slot > 5)
            return;

        int index = slot - 1;

        if (characterSlots[index].color.a == 0)
            characterSlots[index].color = Color.white;

        characterSlots[index].sprite = Resources.Load<Sprite>($"Img/{res}");
    }


    private string GetLocalizedText(string key)
    {
        if (!strTable.Data.TryGetValue(key, out var row))
        {
            Debug.LogWarning($"StringTable: Key '{key}' not found.");
            return $"[{key}]";
        }

        return currentLanguage switch
        {
            Language.Ko => row.ko,
            Language.En => row.en,
            _ => row.ko
        };
    }
    public enum Language
    {
        Ko,
        En
    }
}