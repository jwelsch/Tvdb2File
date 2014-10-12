using System;
using System.IO;
using System.Reflection;

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

      public string ActualPath
      {
         get;
         private set;
      }

      public LocalStoragePath( string fileName )
      {
         this.Default = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), fileName );

         var appDataPath = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
         var tvdb2fileAppDataPath = Path.Combine( appDataPath, "Tvdb2File" );
         this.AppData = Path.Combine( tvdb2fileAppDataPath, fileName );

         this.DeterminePath();
      }

      public bool DoesDefaultExist()
      {
         return File.Exists( this.Default );
      }

      public bool DoesAppDataExist()
      {
         return File.Exists( this.AppData );
      }

      public bool DoesActualPathExist()
      {
         return File.Exists( this.ActualPath );
      }

      private void DeterminePath()
      {
         if ( this.DoesDefaultExist() )
         {
            this.ActualPath = this.Default;
            return;
         }

         if ( this.DoesAppDataExist() )
         {
            this.ActualPath = this.AppData;
            return;
         }

         var testPath = Path.Combine( Path.GetDirectoryName( this.Default ), "test.file" );

         try
         {
            using ( var fileStream = new FileStream( testPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read ) )
            {
               this.ActualPath = this.Default;
            }

            File.Delete( testPath );
         }
         catch ( Exception )
         {
            this.ActualPath = this.AppData;
         }
      }
   }
}
