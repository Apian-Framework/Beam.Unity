using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EthInfoPanel : MonoBehaviour
{
    private float pollSecs = .5f;
    private EthereumProxy eth = null;

    private System.Numerics.BigInteger _prevBlock = 0;

    public GameObject nodeDisp;
    public GameObject blockDisp;

    private float secsSinceLastPoll = 0;
    // Start is called before the first frame update
    void Start()
    {
        eth = OldGameMain.GetInstance().eth;
    }

    // Update is called once per frame
    async void Update()
    {
        secsSinceLastPoll += Time.deltaTime;

        if (eth == null)
            eth = OldGameMain.GetInstance().eth;

        if ( (eth != null) && (eth.web3 != null)  && (secsSinceLastPoll >= pollSecs))
        {   
            secsSinceLastPoll = 0;
            string node = new Uri(eth.ethereumNodeUrl).Host;
            var blockRes = await eth.web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            System.Numerics.BigInteger block = blockRes.Value;          
            if (block > _prevBlock) { // polling will sometimes result in a previous block
                _prevBlock = block;
                nodeDisp.GetComponent<TextMeshPro>().text = string.Format("<b>Eth Node:</b> {0}",node);
                blockDisp.GetComponent<TextMeshPro>().text = string.Format("<b>Block:</b> {0}",block);            
            }
        }

    }
}
