using System;
using System.Collections.Generic;
using System.IO;

namespace Tvdb2File
{
   public class StepGetEpisodesLocally : IExecutionStep
   {
      #region IExecutionStep Members

      public void Execute( ExecutionContext context )
      {
         var episodeList = new List<Episode>();

         using ( var database = new SqliteDatabase() )
         {
            if ( File.Exists( database.DatabasePath ) )
            {
               database.Open( context.LocalStoragePath );

               database.BeginTransaction();

               if ( context.CommandLine.SeriesId == CommandLine.NoSeriesId )
               {
                  var searchTerms = ( !String.IsNullOrEmpty( context.CommandLine.SeriesSearchTerms ) ) ? context.CommandLine.SeriesSearchTerms : context.InputPathInfo.SeriesName;
                  var episodes = database.FindEpisodes( searchTerms, context.InputPathInfo.SeasonNumber );

                  if ( episodes != null )
                  {
                     episodeList.AddRange( episodes );
                  }
               }
               else
               {
                  var episodes = database.FindEpisodes( context.CommandLine.SeriesId, context.InputPathInfo.SeasonNumber );
                  episodeList.AddRange( episodes );
               }
            }
            else
            {
               database.Open( context.LocalStoragePath );

               database.CreateTableSeries();
               database.CreateTableSeason();
               database.CreateTableEpisode();
            }

            database.EndTransaction();
         }

         return episodeList;
      }

      #endregion
   }
}
