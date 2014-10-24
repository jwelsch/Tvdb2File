using System;

namespace Tvdb2File
{
   class StepPurgeLocalData : IExecutionStep
   {
      #region IExecutionStep Members

      public void Execute( ExecutionContext context )
      {
         using ( var database = new SqliteDatabase() )
         {
            database.Open( context.LocalStoragePath );
            database.DeleteAllEpisodes();
            database.DeleteAllSeasons();
            database.DeleteAllSeries();
         }
      }

      #endregion
   }
}
