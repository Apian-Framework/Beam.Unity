const { Libp2p,  Websockets, WebRTCStar,  NOISE,  Mplex,  Bootstrap,  PubsubPeerDiscovery, GossipSub,  FloodSub,
      uint8arrayFromString, uint8arrayToString, PeerId, createFromB58String }  = window.Libp2pObj;

// String constants that might need replacing
// These must be that same as the corresponding C# definitions
const  Websockets_Lbl = "Websockets-inst", WebRTCStar_Lbl = "WebRTCStar-inst" // Transport modules
const  NOISE_Lbl = "NOISE-inst"  // encryption
const  Mplex_Lbl = "Mplex-inst"  // stream multiplexer
const  Bootstrap_Lbl = "Bootstrap-inst"  // PeerDiscovery  modules
const  PubsubPeerDiscovery_Lbl = "PubsubPeerDiscovery-inst"

const  GossipSub_Lbl = "GossipSub-inst", Floodsub_Lbl = "Floodsub-inst" // pubsub modules


class Libp2pConfigError extends Error {
  constructor(message) {
      super(message);
      this.name = "Libp2pConfigError";
  }
}


function CheckConfigTags()
{
  // in the "config section" of the config file (yeah, I know) there are places where a
  // static class "tag" string is used as a key ( BootStrap.Tag ) but as it turns out they
  // really can just be treated as constants (ie. Bootstrap.tag is always "bootstrap")
  // Now - this isn;t toally safe, of course, but it's WAY simpler if in the C# code we
  // can just use em that way.

  // Anyways - the idea here is to make sure that any that we actually use in C# are still what we think they are...
  if (Bootstrap.tag !== "bootstrap") // used to config bootstrap PeerDiscovery
    throw new Libp2pConfigError("Bootstrap.tag is not 'bootstrap'");

  if (PubsubPeerDiscovery.tag !== "PubsubPeerDiscovery")
    throw new Libp2pConfigError("PubsubPeerDiscovery.tag is not 'PubsubPeerDiscovery'");
}


function ConfigAddresses(inAddresses)
{
  var addresses = {}

  if (inAddresses.hasOwnProperty('listen'))
    addresses.listen = inAddresses.listen  // both are string lists

  if (inAddresses.hasOwnProperty('announce'))
    addresses.announce = inAddresses.announce

  if (inAddresses.hasOwnProperty('announceFilter')) {
    throw new Libp2pConfigError(`addresses.announceFilter not supported.`)
  }

  return addresses
}

function ConfigTransports(inConf)
{
  // inConf is a list of transport labels
  var transportInstances = {
    [Websockets_Lbl]: Websockets,
    [WebRTCStar_Lbl]: WebRTCStar
  }

  var transport = []

  inConf.forEach( (label) => {
    if (transportInstances.hasOwnProperty(label))
      transport.push(transportInstances[label])
    else
      throw new Libp2pConfigError(`transport "${label}" not supported`)
  })

  if (transport.length === 0)
      throw new Libp2pConfigError(`No supported transports in \"${JSON.stringify(inConf)}\"`)

  return transport
}

function ConfigEncryption(inConf)
{
  // inConf is a list [] of encryptions
  var encryptionIstances = {
    [NOISE_Lbl]: NOISE
  }

  // TODO: if unspecified shoud it default to NOISE?
  var connEncryption = []
  inConf.forEach( (label) => {
    if (encryptionIstances.hasOwnProperty(label))
      connEncryption.push(encryptionIstances[label])
    else
      throw new Libp2pConfigError(`encryption "${label}" not supported`)
  })
  if (connEncryption.length === 0)
    throw new Libp2pConfigError(`No supported encryption modules in \"${JSON.stringify(inConf)}\"`)

  return connEncryption
}


function ConfigMux(inConf)
{
  // TODO: if unspecified shoud it default to Mplex?
  var streamMuxer = []

  if (inConf.includes(Mplex_Lbl))
    streamMuxer.push(Mplex)
  else
    throw new Libp2pConfigError(`No supported multiplex modules in \"${JSON.stringify(inConf)}\"`)

  return streamMuxer
}

function ConfigPeerDiscovery(inConf)
{
  var discoveryInstances = {
    [Bootstrap_Lbl]: Bootstrap,
    [PubsubPeerDiscovery_Lbl]: PubsubPeerDiscovery
  }

  var peerDiscovery = []

  inConf.forEach( (label) => {
    if (discoveryInstances.hasOwnProperty(label))
    peerDiscovery.push(discoveryInstances[label])
    else
      throw new Libp2pConfigError(`Peer discovery "${label}" not supported`)
  })

  if (peerDiscovery.length === 0)
      throw new Libp2pConfigError(`No supported peer dicovery methods in \"${JSON.stringify(inConf)}\"`)

  return peerDiscovery
}

function ConfigPubsub(inConf)
{
  var pubsub = null
  if (inConf === Floodsub_Lbl)
    pubsub = FloodSub
  else if (inConf === GossipSub_Lbl)
    pubsub = GossipSub
  else
    throw new Libp2pConfigError(`unsupported pubsub module: ${inConf}`)

  return pubsub
}

function ConfigModules( inputConfig )
{
  //
  var moduleHandlers = {
    'transport': ConfigTransports,
    'connEncryption': ConfigEncryption,
    'streamMuxer': ConfigMux,
    'peerDiscovery': ConfigPeerDiscovery,
    'pubsub': ConfigPubsub
  }

  var modules = {}

  if (!inputConfig.hasOwnProperty('modules'))
    throw new Libp2pConfigError(`no "modules" config section` )


  var moduleList = Object.keys(inputConfig.modules)
  moduleList.forEach( moduleName => {
    // Go through each module liste in the input config.
    if (moduleHandlers.hasOwnProperty(moduleName))
    {
      var newConf = moduleHandlers[moduleName](inputConfig.modules[moduleName])
      if (newConf)
        modules[moduleName] = newConf
    } else
      throw new Libp2pConfigError(`module "${moduleName}" is not currently supported`)
  })

  var requiredModules = ['transport', 'connEncryption', 'streamMuxer']
  requiredModules.forEach( moduleName => {
    if (!modules.hasOwnProperty(moduleName))
      throw new Libp2pConfigError(`Required module "${moduleName}" is not in configuration`)
  })

  return modules
}


export function ProcessConfig( inputConfig)
{
  CheckConfigTags() // just to make sure

  var config = {}

  // Listen addresses
  if (inputConfig.hasOwnProperty('addresses')) // not required
    config.addresses = ConfigAddresses(inputConfig.addresses)

  // Modules
  config.modules = ConfigModules(inputConfig);

  // Cofiguration data
  // This currenly assumes that the "config" section just consists of string keys,
  // and string, numeric, and boolean values so can just be copied as-is into the configuration
  if (inputConfig.hasOwnProperty('config'))
    config.config = inputConfig.config

  // PeerID - must include privKey or the whole thing doesn;t work
  if (inputConfig.hasOwnProperty('peerId'))
    config.config = inputConfig.config
  return config;
}

export var Libp2p_configs = {

  WebRtcStar_Bs_Gossip_config: {
    addresses: {
      // Add the signaling server address, along with our PeerId to our multiaddrs list
      // libp2p will automatically attempt to dial to the signaling server so that it can
      // receive inbound connections from other peers
      listen: [
        //'/dns4/newsweasel.com/tcp/443/wss/p2p-webrtc-star/',
        '/dns4/wrtc-star1.par.dwebops.pub/tcp/443/wss/p2p-webrtc-star',
        '/dns4/wrtc-star2.sjc.dwebops.pub/tcp/443/wss/p2p-webrtc-star'
      ]
    },
    modules: {
      transport: [Websockets, WebRTCStar],
      connEncryption: [NOISE],
      streamMuxer: [Mplex],
      peerDiscovery: [Bootstrap],
      pubsub: GossipSub
    },
    config: {
      pubsub: {
        enabled: true,
        emitSelf: false
      },
      peerDiscovery: {
        // The `tag` property will be searched when creating the instance of your Peer Discovery service.
        // The associated object, will be passed to the service when it is instantiated.
        [Bootstrap.tag]: {
          enabled: false,
          list: [
            // '/dnsaddr/bootstrap.libp2p.io/p2p/QmNnooDu7bfjPFoTZYxMNLWUQJyrVwtbZg5gBMjTezGAJN',
            // '/dnsaddr/bootstrap.libp2p.io/p2p/QmbLHAnMoJPWSCR5Zhtx6BHJX9KiKNN6tpvbUcqanj75Nb',
            // '/dnsaddr/bootstrap.libp2p.io/p2p/QmZa1sAxajnQjVM8WjWXoMbmPd7NsWhfKsPkErzpm9wGkp',
            // '/dnsaddr/bootstrap.libp2p.io/p2p/QmQCU2EcMqAqQPR2i9bChDtGNJchTbq5TbXJJ16u19uLTa',
            // '/dnsaddr/bootstrap.libp2p.io/p2p/QmcZf59bWwK5XFi76CZX8cbJ4BhTzzA3gU1ZjYZcYW3dwt'
          ]
        }
      }
    }
  },

  WebSocket_Bs_config: {
    modules: {
      transport: [Websockets],
      connEncryption: [NOISE],
      streamMuxer: [Mplex],
      peerDiscovery: [Bootstrap]
    },
    config: {
      relay: {
        enabled: true,
        autoRelay: {
          enabled: true,
          maxListeners: 2
        }
      },
      peerDiscovery: {
        autoDial: true,
        [Bootstrap.tag]: {
           enabled: true,
           list: [
            '/dns4/newsweasel.com/tcp/15003/wss/p2p/12D3KooWArqNU1injda6AGUVzpSVciQXFLuRWnjtmF2sppLy6Tc8',
      //       //'/dnsaddr/newsweasel.com/p2p/12D3KooWArqNU1injda6AGUVzpSVciQXFLuRWnjtmF2sppLy6Tc8'
      //       //'/ip4/64.227.18.159/tcp/43237/ws/p2p/12D3KooWArqNU1injda6AGUVzpSVciQXFLuRWnjtmF2sppLy6Tc8'
      //       //'/ip4/10.10.0.5/tcp/43237/ws/p2p/12D3KooWArqNU1injda6AGUVzpSVciQXFLuRWnjtmF2sppLy6Tc8'
      //       // '/dnsaddr/bootstrap.libp2p.io/p2p/QmNnooDu7bfjPFoTZYxMNLWUQJyrVwtbZg5gBMjTezGAJN',
      //       // '/dnsaddr/bootstrap.libp2p.io/p2p/QmbLHAnMoJPWSCR5Zhtx6BHJX9KiKNN6tpvbUcqanj75Nb',
      //       // '/dnsaddr/bootstrap.libp2p.io/p2p/QmZa1sAxajnQjVM8WjWXoMbmPd7NsWhfKsPkErzpm9wGkp',
      //       // '/dnsaddr/bootstrap.libp2p.io/p2p/QmQCU2EcMqAqQPR2i9bChDtGNJchTbq5TbXJJ16u19uLTa',
      //       // '/dnsaddr/bootstrap.libp2p.io/p2p/QmcZf59bWwK5XFi76CZX8cbJ4BhTzzA3gU1ZjYZcYW3dwt'
           ]
        }
      }
    }
  }

}

