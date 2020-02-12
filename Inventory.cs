using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// アイテムナンバー0のアイテムは存在しません。
/// </summary>
public class Inventory : MonoBehaviour

{
    public List<int> CaseNumber = new List<int>(); //インベントリケースに格納されているアイテムの番号
    public List<int> CaseIndex = new List<int>(); //インベントリケースに格納されているアイテムの数
    public List<bool> CaseTF = new List<bool>(); //インベントリケースに任意のアイテムが格納されているか
    public List<GameObject> Cases = new List<GameObject>(); //複製されたインベントリケースが格納される
    private int inventoryopenjudge;
    private int Localdex;
    private int selecteddex;
    private int equippedweapon;
    private int[] equippedarmors = new int[5];
    private bool Accept;
    private bool interference;
    private bool islcpush;
    public GameObject InventoryObject;
    public GameObject Case;
    public GameObject ItemChooseFase;
    public GameObject AcceptButton;
    public XDocument xml;
    public GameObject Canvas;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 17; i++)
        {
            CaseNumber.Add(0);
            CaseIndex.Add(0);
            CaseTF.Add(false);
        }
        InventoryObject.gameObject.SetActive(false);
        xml = XDocument.Load(@"D:\Inventory Developing\Assets\ItemData.xml");
      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            islcpush = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            islcpush = false;
        }
    }

    //!!
    [System.Obsolete("Warning! 関数使用例！", true)]
    public void EXSetItemCommand()
    {
        //後述関数のSetItemCommandTemprateの使用例です
        SetItemCommandTemprate(itemnumber: 1, itemindex: 1);
    }
    //!!

    public void OpenInventoryButton()
    {
        //インベントリマークのオンクリック関数です。
        inventoryopenjudge++;
        if (inventoryopenjudge % 2 == 1)
        {
            InventoryObject.gameObject.SetActive(true);
        }
        else
        {
            InventoryObject.gameObject.SetActive(false);
        }
    }

    public void SetItemCommandTemprate(int itemnumber, int itemindex)
    {
        for (int i = 0; i < CaseNumber.Count; i++)
        {
            if (!(CaseNumber[i] == itemnumber))
            {
                //今回追加された任意のアイテムは新規ナンバーのアイテムです。新しくケースを生成します。
                GeneralCase(itemnumber: itemnumber, itemindex: itemindex);
            }
            else
            {
                //すでに所持している既存ナンバーのアイテムなので自動で追加されます。
                CaseIndex[i] = CaseIndex[i] + itemindex;
            }


        }
        //インベントリに任意のアイテムをシステム側から追加するときに使用しましょう。

    }
    public void GeneralCase(int itemnumber, int itemindex)
    {
        //インベントリに新規の任意なアイテムが追加された場合、新しいスロットを追加する関数です。
        for (int i = 0; i < CaseTF.Count; i++)
        {
            if (CaseTF[i] == false)
            {
                GameObject prefab = Instantiate(Case);
                int localdex = i;
                prefab.transform.SetParent(Canvas.transform,false);
                //ボタンの押下時、イベントの引数にケース番号を設定します。
                prefab.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnClickCase(localdex);
                });
                int setindexheight = 600 - i / 6 * 40;
                int setindexwidth = 500 - i / 6 * 40;
                //新しいスロットをboolの順番で判定して指定の場所へ生成します。
                CaseTF[i] = true;
                CaseNumber[i] = itemnumber;
                CaseIndex[i] = itemindex;
                Cases.Add(prefab);
            }
        }
    }
    
    void OnClickCase(int dex)
    {
        Debug.Log("ケースが押されました。");
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Debug.Log("アイテム選択数決定フェーズに入ります。");
            //アイテム選択数決定フェーズに入ります。
            GameObject prefab = Instantiate(ItemChooseFase);
            prefab.transform.SetParent(Canvas.transform, false);
            prefab.GetComponent<InputField>().onValueChanged.AddListener((string arg) =>
           {
               OnInputEnd(int.Parse(arg));
           });
            
            ItemChooseFase.transform.position = new Vector3(0, 0, 0);
            GameObject button = Instantiate(AcceptButton);
            button.transform.SetParent(prefab.transform, true);
            button.transform.localPosition = new Vector3(20, -10, 0);
            int clickdex = dex;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnAcceptClick(clickdex);
            });
            Localdex = dex;
        }
        //通常での処理となります。
        ItemNumberClassification(1, CaseNumber[dex]);
    }
     public void OnInputEnd(int arg)
    {
        if (arg <= 0 || arg > CaseIndex[Localdex])
        {
            Debug.Log("選択したアイテムの数は0個以下かもしくはそのアイテムの所持数を超えているため、イベントはキャンセルされました。");
        }
        else
        {
            selecteddex = arg;
            Accept = true;
        }
    }
    public  void OnAcceptClick(int dex)
    {
        if (Accept == true)
        {
            Debug.Log("Test");
            //後記の関数のlocal関数のdexに対してアイテム選択数、numberに対してアイテムナンバーを代入します。
            ItemNumberClassification(selecteddex,CaseNumber[dex] );
            //選択フェーズを消去

            Accept = false;
        }
    }
    void ItemNumberClassification(int dex, int number)
    {
        Debug.Log("Test!!");
        if (number > 0){
            LoadItemData(dex, number);
        }
    }
    void LoadItemData(int dex, int number)
    {
        Debug.Log("XMLまで");
        XElement table = xml.Element("Item");
        var rows = table.Elements("ItemNumber");
        foreach(XElement row in rows)
        {
            Debug.Log("foreachまで");
            XElement item = row.Element(number.ToString());
            if (interference == false)
            {
                Debug.Log("干渉チェックまで");
                if (item.Elements("ItemType").ToString() == "Weapon")
                {
                    //任意のItemNumberのタイプが"Weapon"です
                    CaseIndex[number] = CaseIndex[number] - 1;
                    if (CaseIndex[number] <= 0)
                    {
                        equippedweapon = number;
                        Debug.Log(item.Elements("ItemName") + "を装備したよ");
                        //残りのアイテム数が0となりました => ケース削除、ケース際昇順配列化
                    }
                    if (CaseIndex[equippedweapon] > 0)
                    {
                        CaseIndex[equippedweapon] = CaseIndex[equippedweapon] + 1;
                        equippedweapon = number;
                        Debug.Log(item.Elements("ItemName") + "を装備したよ");
                    }
                    equippedweapon = number;
                    Debug.Log(item.Elements("ItemName") + "を装備したよ");

                }
                if (item.Elements("ItemType").ToString() == "Armor")
                {
                    //任意のItemNumberのタイプが"Aromor"です
                }
                if (item.Elements("ItemType").ToString() == "Resource")
                {
                    //任意のItemNumberのタイプが"Resource"です
                }
            }
            else
            {
                //干渉インベントリが開かれている際の実行です。
                
            }
        }
    }
}
