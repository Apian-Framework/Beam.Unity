using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Web3;
using UnityEngine;

public class EthObj : MonoBehaviour
{
    public static EthObj instance {get; private set; } = null;  

    public string ethereumNodeURL {get; private set; } = "https://mainnet.infura.io";
    public Web3 web3 {get; private set; } = null;  
  
    // MonoBehavior lifecycle

    //Awake is always called before any Start functions
    async void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);    
            await Task.Run(() => ConnectToNode());              
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            Destroy(gameObject);   
        }

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {        
    }

    // Utility

    private void ConnectToNode()
    {
        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
        web3 = new Web3(ethereumNodeURL); //defaults to http://localhost:8545        
    }

    public void DoExitGame() 
    {
        Debug.Log("Yikes!");        
        Application.Quit();
    }    

    private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
    {
        // all certificates are accepted
        return true;
    }

}
