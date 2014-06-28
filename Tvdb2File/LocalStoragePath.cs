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
         var tvdb2fileAppDataPath = Path.Combine( appDataPath, "Tvdb2File" );
         this.AppData = Path.Combine( tvdb2fileAppDataPath, fileName );
      }

      public bool DoesDefaultExist()
      {
         return File.Exists( this.Default );
      }

      public bool DoesAppDataExist()
      {
         return File.Exists( this.AppData );
      }
   }
}
