using System;
using System.IO;
using System.Collections.Generic;

namespace Tvdb2File
{
   public class LocalStoreClient
   {
      private LocalStoragePath storagePath;

      public LocalStoreClient( LocalStoragePath storagePath )
      {
         this.storagePath = storagePath;
      }

      public IList<Episode> GetEpisodesLocally( CommandLine commandLine, InputPathInfo inputPathInfo )
      {
         var episodeList = new List<Episode>();

         using ( var database = new SqliteDatabase() )
         {
            database.Open( this.storagePath );

            if ( File.Exists( Program.LocalDatabasePath ) )
            {
               database.BeginTransaction();

               if ( commandLine.SeriesId == CommandLine.NoSeriesId )
               {
                  var searchTerms = ( !String.IsNullOrEmpty( commandLine.SeriesSearchTerms ) ) ? commandLine.SeriesSearchTerms : inputPathInfo.SeriesName;
                  var episodes = database.FindEpisodes( searchTerms, inputPathInfo.SeasonNumber );

                  if ( episodes != null )
                  {
                     episodeList.AddRange( episodes );
                  }
               }
               else
               {
                  var episodes = database.FindEpisodes( commandLine.SeriesId, inputPathInfo.SeasonNumber );
                  episodeList.AddRange( episodes );
               }
            }
            else
            {
               database.CreateTableSeries();
               database.CreateTableSeason();
               database.CreateTableEpisode();
            }

            database.EndTransaction();
         }

         return episodeList;
      }

      public void StoreEpisodesLocally( CommandLine commandLine, InputPathInfo inputPathInfo, IList<Episode> episodeList )
      {
         var count = 0;

         using ( var database = new SqliteDatabase() )
         {
            database.Open( this.storagePath );
            database.BeginTransaction();

            var seriesCache = new Dictionary<Int64, Series>();
            var seasonCache = new Dictionary<Int64, Season>();

            Series series = null;
            Season season = null;

            foreach ( var newEpisode in episodeList )
            {
               var lookUpEpisode = true;

               if ( !seriesCache.TryGetValue( newEpisode.SeriesId, out series ) )
               {
                  series = database.FindSeries( newEpisode.SeriesId );

                  if ( series == null )
                  {
                     series = new Series()
                     {
                        Name = inputPathInfo.SeriesName,
                        SeriesId = newEpisode.SeriesId
                     };

                     database.InsertSeries( series );
                     lookUpEpisode = false;
                  }

                  seriesCache.Add( series.SeriesId, series );
               }

               if ( !seasonCache.TryGetValue( newEpisode.SeasonId, out season ) )
               {
                  season = database.FindSeason( newEpisode.SeriesId, newEpisode.SeasonId );

                  if ( season == null )
                  {
                     season = new Season()
                     {
                        SeasonId = newEpisode.SeasonId,
                        SeasonNumber = newEpisode.SeasonNumber,
                        SeriesId = newEpisode.SeriesId
                     };

                     database.InsertSeason( season );
                     lookUpEpisode = false;
                  }

                  seasonCache.Add( season.SeasonId, season );
               }

               Episode storedEpisode = null;

               if ( lookUpEpisode )
               {
                  storedEpisode = database.FindEpisode( series.SeriesId, season.SeasonNumber, newEpisode.EpisodeNumber );
               }

               if ( storedEpisode == null )
               {
                  database.InsertEpisode( newEpisode );

                  count++;

                  if ( count % 10 == 0 )
                  {
                     Console.Write( String.Format( "\rAdded {0} episodes to local store.", count ) );
                  }
               }
            }

            database.EndTransaction();
         }

         Console.WriteLine( String.Format( "\rAdded {0} episodes to local store.", count ) );
      }

      public void PurgeLocalData()
      {
         using ( var database = new SqliteDatabase() )
         {
            database.Open( this.storagePath );
            database.DeleteAllEpisodes();
            database.DeleteAllSeasons();
            database.DeleteAllSeries();
         }
      }
   }
}
