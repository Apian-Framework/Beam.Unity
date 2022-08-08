
import {Libp2p_configs, ProcessConfig} from './js-libp2p-configs.js'

const { Libp2p,  Websockets, WebRTCStar,  NOISE,  Mplex,  Bootstrap,  PubsubPeerDiscovery, GossipSub,  FloodSub,
    uint8arrayFromString, uint8arrayToString, PeerId, createFromB58String }  = window.Libp2pObj;

var JslibApiCallbacks = { }

var ClientContexts = { }

function EquivArrays(arrA, arrB)
{
  return ( arrA.length == arrB.length
    && arrA.every(function (element) {
      return arrB.includes(element);
    })
  )
}

function EquivSets(setA, setB)
{
  return ( setA.size == setB.size
    && [...setA].every(function (element) {
      return setB.has(element);
    })
  )
}

function PeerIdIfNotMultiAddr(maddrOrId)
{
  // Assumes it's either a B58 id string, or a multiaddress
  return maddrOrId.charAt(0) === '/' ? maddrOrId : createFromB58String(maddrOrId)
}


export default  {

  InitApiCallbacks: function( OnLibCreated_Cb, OnLibStarted_Cb,  OnDiscovery_Cb,  OnConnection_Cb,
      OnListenAddr_Cb, OnMessage_Cb, OnLibStopped_Cb, OnPing_Cb )
  {
    JslibApiCallbacks.OnLibCreated = OnLibCreated_Cb
    JslibApiCallbacks.OnLibStarted = OnLibStarted_Cb
    JslibApiCallbacks.OnDiscovery = OnDiscovery_Cb
    JslibApiCallbacks.OnConnection = OnConnection_Cb
    JslibApiCallbacks.OnListenAddress = OnListenAddr_Cb
    JslibApiCallbacks.OnMessage = OnMessage_Cb
    JslibApiCallbacks.OnLibStopped = OnLibStopped_Cb
    JslibApiCallbacks.OnPing = OnPing_Cb
  },

  CreateFromNamedCfg : async function(clientId, cfgName, cfgOpts)
  {
    this.Create(clientId, Libp2p_configs[cfgName], cfgOpts);
  },

  CreateFromConfig : async function(clientId, configIn)
  {
    var fullConfig = ProcessConfig(configIn)
    this.Create(clientId, fullConfig);
  },

  Create : async function(clientId, config)
  {
    console.log(`CreateLib(): Type of passed-in clientID: ${typeof(clientId)} `)

    var libp2pInst = await Libp2p.create( config )

    var ClientContext =  {
      version: 2,
      clientId: clientId,
      libp2pInst: libp2pInst,
      subscribedTopics: [],
      listenMaddrs: [],
      msgHandlers: {} // ctx.msgHandlers[topic]
    }
    ClientContexts[clientId] = ClientContext;
		console.log(`Created Libp2p instance: ${clientId} LocalPeerId: ${libp2pInst.peerId.toB58String()}`)

    JslibApiCallbacks.OnLibCreated(clientId, libp2pInst.peerId);
  },

  StartLib : async function(clientId)
  {
      var ctx = ClientContexts[clientId];

      // Listen for discovered peers
      ctx.libp2pInst.on('peer:discovery', (remotePeer) => {
        const peerIdStr = remotePeer.toB58String();
        console.log('Discovered %s', peerIdStr) // Log discovered peer
        JslibApiCallbacks.OnDiscovery(clientId, remotePeer)
      })

      // Listen for new connections to peers
      ctx.libp2pInst.connectionManager.on('peer:connect', (connection) => {
        const peerIdStr = connection.remotePeer.toB58String();
        console.log(`Inst ${clientId} Connected to ${peerIdStr}`)
        JslibApiCallbacks.OnConnection(clientId, connection.remotePeer, true)
      })

      // Listen for peers disconnecting
      ctx.libp2pInst.connectionManager.on('peer:disconnect', (connection) => {
        const peerIdStr = connection.remotePeer.toB58String()
        console.log(`Disconnected from ${peerIdStr}`)
        JslibApiCallbacks.OnConnection(clientId, connection.remotePeer,false)
      })

      // If we bind to a relay or sentinel in order to be able to have a "listen" address,
      // this will fire with the peerId being our local peerid, and we need to report this
      // back to the client app to tell it what our "listen" address(es) is/are
      ctx.libp2pInst.peerStore.on('change:multiaddrs', async ({ peerId }) => {
        // Updated multiaddrs for this local node?
        if (peerId.equals(ctx.libp2pInst.peerId)) {
          var maddrs = await ctx.libp2pInst.peerStore.addressBook.getMultiaddrsForPeer(peerId)
          var listenAddrs = maddrs.map( maddr => maddr.toString() )
          // This will get called when the addresses are still the same, so make sure they've changed
          if ( !EquivArrays(ctx.listenMaddrs, listenAddrs)) {
            ctx.listenMaddrs = listenAddrs;
            console.log(`Advertising listen addresses of ${listenAddrs}`)
            JslibApiCallbacks.OnListenAddress(clientId,listenAddrs)
          }
        }
      })

      await ctx.libp2pInst.start()

      JslibApiCallbacks.OnLibStarted(clientId);

    },

    StopLib : async function(clientId)
    {
        var ctx = ClientContexts[clientId]

        await ctx.libp2pInst.stop()

        // TODO: Actuallly, I don't really know what happens during js-libp2p.stop()
        // I dunno what happens if you just re-start() it...
        // I suspect I oughta clean up the clientContext vars other than clientId and libInst

        // TODO: probably should unsubscribe from topics here? Does stop() do it?

        JslibApiCallbacks.OnLibStopped(clientId)
    },

    Subscribe : function(clientId, topic)
    {
      // return true on success
      var ctx = ClientContexts[clientId]
      if (ctx.subscribedTopics.includes(topic))
      {
        console.log(`Subscribe(): Inst ${clientId} already subscribed to topic: ${topic}`)
        return false
      }

      // Need to assign the bound handler to a ctx property so we can
      ctx.libp2pInst.pubsub.on( topic, ctx.msgHandlers[topic] = (msg) => {
          // msg is: { from: string, data: Uint8Array, seqno: Uint8Array, topicIDs: Array<string>, signature: Uint8Array, key: Uint8Array }
          //console.log(`Got pubsub msg. From: ${msg.from} Topic: ${topic} Payload: ${uint8arrayToString(msg.data)}`)
          JslibApiCallbacks.OnMessage( ctx.clientId, msg.from, topic, uint8arrayToString(msg.data) )
      })
      ctx.libp2pInst.pubsub.subscribe(topic)
      ctx.subscribedTopics.push(topic)
      return true
    },

    Unsubscribe : function(clientId, topic)
    {
      // return true on success
      var ctx = ClientContexts[clientId]
      if (!ctx.subscribedTopics.includes(topic))
      {
        console.log(`Unsubscribe(): Inst ${clientId} NOT subscribed to topic: ${topic}`)
        return false
      }

      ctx.subscribedTopics = ctx.subscribedTopics.filter( (t) => t !== topic )
      ctx.libp2pInst.pubsub.unsubscribe(topic)
      ctx.libp2pInst.pubsub.removeListener( topic, ctx.msgHandlers[topic])
      delete ctx.msgHandlers[topic]
      return true
    },

    Publish : async function(clientId, topic, message)
    {
      var ctx = ClientContexts[clientId]
      const data = uint8arrayFromString(message)
      await ctx.libp2pInst.pubsub.publish(topic, data)
    },

    Dial : async function(clientId, addr)
    {
      var ctx = ClientContexts[clientId]
      var target = PeerIdIfNotMultiAddr(addr)
      await ctx.libp2pInst.dial(target)
      // reports back via connection callbacks
    },

    HangUp : async function(clientId, addr)
    {
      var ctx = ClientContexts[clientId]
      var target = PeerIdIfNotMultiAddr(addr)
      await ctx.libp2pInst.hangUp(target)
      // reports back via connection callbacks
    },

    Ping : async function(clientId, addr)
    {
      var ctx = ClientContexts[clientId]
      var target = PeerIdIfNotMultiAddr(addr)
      var latencyMs = await ctx.libp2pInst.ping(target)
      JslibApiCallbacks.OnPing(clientId, addr, latencyMs)
    }

 }
