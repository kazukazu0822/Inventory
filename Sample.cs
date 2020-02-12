using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Sample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            this.GetComponent<Inventory>().SetItemCommandTemprate(1, 1);
            Debug.Log("1,1のアイテムが追加されました。");
        }
    }
}
