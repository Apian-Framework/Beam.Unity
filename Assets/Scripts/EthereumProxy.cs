using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Web3;
using UnityEngine;

public class EthereumProxy
{
    // public static EthereumProxy instance {get; private set; } = null;  

    public static readonly string  InfuraRinkebyUrl = "https://rinkeby.infura.io";
    public static readonly string InfuraMainnetUrl = "https://mainnet.infura.io";
    public Web3 web3 {get; private set; } = null;  

    public string ethereumNodeUrl {get; private set; } = null;
  
    public EthereumProxy() 
    {
        ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
    }

    async public void ConnectAsync(string url)
    {
        Debug.Log("Connecting...");
        await Task.Run(() => Connect(url));              
        Debug.Log("Connect Done.");
    }

    public  void Connect(string url)
    {
        web3 = new Web3(url);
        if (web3 != null)
            ethereumNodeUrl = url; 
        Debug.Log(string.Format("Eth Node: {0}", ethereumNodeUrl));
    }

    private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors)
    {
        // all certificates are accepted
        return true;
    }

}
