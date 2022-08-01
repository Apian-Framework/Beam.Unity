using UnityEngine;
using System;
using BeamGameCode;
using P2pNet;

public class UnityBeamGameNet : BeamGameNet
{
    public UnityBeamGameNet() : base()
    {

    }

    protected override IP2pNet P2pNetFactory(string p2pConnectionString)
    {
        // P2pConnectionString is <p2p implmentation name>::<imp-dependent connection string>
        // Names are: p2ploopback, p2predis

        IP2pNetCarrier carrier = null;

        string[] parts = p2pConnectionString.Split(new string[]{"::"},StringSplitOptions.None); // Yikes! This is fugly.

        switch(parts[0])
        {
            case "p2ploopback":
                carrier = new P2pLoopback(null);
                break;
#if  UNITY_WEBGL && !UNITY_EDITOR
            case "p2punitylibp2p":
            carrier = new P2pNetLibp2p(parts[1]);
            break;
#else
            case "p2predis":
                carrier = new P2pRedis(parts[1]);
                break;

            case "p2pmqtt":
                carrier = new P2pMqtt(parts[1]);
                break;
            // case "p2pactivemq":
            //     carrier = new P2pActiveMq(parts[1]);
            //     break;
#endif
            default:
                throw( new Exception($"Invalid connection type: {parts[0]}"));
        }

        IP2pNet ip2p = new P2pNetBase(this, carrier);

        // TODO: Since C# ctors can't fail and return null we don;t have a generic
        // "It didn;t work" path. As it stands, the P2pNet ctor will throw and we'll crash.
        // That's probably OK for now - but this TODO is here to remind me later.

        return ip2p;
    }



}
