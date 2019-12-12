using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StackExchange.Redis;

public class RedisTester : MonoBehaviour
{
    public string connectionString = "sparkyx,password=sparky-redis79,abortConnect=false";
    public string mainChannel = "7890";

    public float msgDelaySecs = 3.0f;

    public ConnectionMultiplexer RedisCon {get; private set; } = null;

    private int updateIter = 0;
    private bool listenCalled = false;
    private float secsSinceMsg = 0; 

    // Start is called before the first frame update
    public async void Start()
    {
        Debug.Log($"**********Redis connecting... ({connectionString})");   
        RedisCon = await ConnectionMultiplexer.ConnectAsync(connectionString);  
        Debug.Log("*********Redis connected.");      
    }

    // Update is called once per frame
    public void Update()
    {
        if (updateIter == 0)
            Debug.Log("*********RedisTester update 0");        

        if (RedisCon != null)
        {
            if(listenCalled == false)
            {
                Debug.Log("*********RedisTester calling _Listen()");
                _Listen(mainChannel);
            }

            if (secsSinceMsg > msgDelaySecs)
            {
                Debug.Log("*********RedisTester sending...");
                _Send(mainChannel, $"Msg from Frame {updateIter}");
                secsSinceMsg = 0;               
            }

            secsSinceMsg += Time.deltaTime;
        }

        updateIter++;        
    }

    protected void _Listen(string channel)
    {
        RedisCon.GetSubscriber().Subscribe(channel, (rcvChannel, msg) => {
            Debug.Log("*********RedisTester: Got message: {msg}");             
        });     
        listenCalled = true;   
    }

    protected void _Send(string channel, string  msg)
    {
        RedisCon.GetSubscriber().Publish(channel, msg);
    }

}
