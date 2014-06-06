using System;
using System.IO;

namespace Tvdb2File
{
   public class LocalStoragePath
   {
      public string Default
      {
         get;
         private set;
      }

      public string AppData
      {
         get;
         private set;
      }

      public LocalStoragePath( string defaultPath )
      {
         this.Default = defaultPath;

         var fileName = Path.GetFileName( this.Default );
         var appDataPath = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
         this.AppData = Path.Combine( appDataPath, fileName );
      }
   }
}
