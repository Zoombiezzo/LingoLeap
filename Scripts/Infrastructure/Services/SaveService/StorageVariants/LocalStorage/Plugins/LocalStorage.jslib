var LocalStorage = {
    SetItemLocalStorage: function(key, value){
      localStorage.setItem(UTF8ToString(key), UTF8ToString(value));
    },

    GetItemLocalStorage: function(key){
      var str = localStorage.getItem(UTF8ToString(key));
      var bufferSize = lengthBytesUTF8(str) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(str, buffer, bufferSize);
      return buffer;
    },

    HasKeyLocalStorage: function(key){
      return localStorage.getItem(UTF8ToString(key)) ? 1 : 0;
    },

    RemoveItemLocalStorage: function(key){
      localStorage.removeItem(UTF8ToString(key));
    },

    ClearLocalStorage: function(){
      localStorage.clear();
    }
};

mergeInto(LibraryManager.library, LocalStorage);