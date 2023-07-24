
var BrowserClip_API = {
    $dependencies:{},

    JS_CopyToBrowserClip: function (c_stringToCopy)
    {
        // returns a bool (yeah, but only to tell whether the async call worked, not the copy itself
        console.log(`CopyToBrowserClip()`)

        if (!navigator.clipboard) {
            console.error(`CopyToBrowserClip(): Browser does not support navigator.clipboard`)
            return false
        }
        var stringToCopy = UTF8ToString(c_stringToCopy)
        navigator.clipboard.writeText(stringToCopy)

        navigator.clipboard.writeText(stringToCopy).then(function() {
            console.log('CopyToBrowserClip(): success');
        }, function(err) {
            console.error('CopyToBrowserClip() failed: ', err);
            return false
        });

        return true
   }


 }
 autoAddDeps(BrowserClip_API,'$dependencies')
 mergeInto(LibraryManager.library,BrowserClip_API)