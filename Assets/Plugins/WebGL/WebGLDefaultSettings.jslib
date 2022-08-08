
var WebGLDefaultSettings_API = {
  $dependencies:{},

  Get_WebGLDefaultSettings: function ()
  {
    var settingsJson = allocate(intArrayFromString(JSON.stringify(WebGLDefaultSettings)), ALLOC_NORMAL)
    return settingsJson;
  }

 }
 autoAddDeps(WebGLDefaultSettings_API,'$dependencies')
 mergeInto(LibraryManager.library,WebGLDefaultSettings_API)
