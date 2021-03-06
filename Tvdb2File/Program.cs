﻿//////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CommandLineLib;

namespace Tvdb2File
{
   class Program
   {
      /*
       * OBJECTIVE
       * Retrieve TV series episode information from thetvdb.com and use it to automatically name files in the local file system.
       * 
       * PARAMETERS
       * - Only one season can be processed at a time.
       * - The app expects a certain directory structure.
       * - The app will only rename MP4 files.
       * - MP4 files are expected to be in the same order as they appear in the season.
       * 
       * FILE SYSTEM STRUCTURE
       * .\
       *  |- Series A
       *            |- Season 1
       *                      |- Episode 1 MP4
       *                      |- ...
       *                      |- Episode n MP4
       *            |- ...
       *            |- Season n
       *  |- Series B
       *            |- Season 1
       *                      |- Episode 1 MP4
       *                      |- ...
       *                      |- Episode n MP4
       *            |- ...
       *            |- Season n
       * 
       * COMMAND LINE ARGUMENTS
       *   Tvdb2File.exe -season "path to season" [[-search "series search term"]|[-seriesId xxxxx]] [-forceUpdate] [-dryRun] [-collapseMultiPart]
       *     -season: (Mandatory) Relative or absolute path to the directory containing the season to rename.
       *     -search: (Optional) Terms to use to search for the series.  If not supplied, the name of the series directory will be used.  This argument is mutually exclusive with -seriesId.
       *     -seriesId: (Optional) ID of the series to use for episode naming.  This argument is mutually exclusive with -search.
       *     -forceUpdate: (Optional) Include to force an update of the local episode database from thetvdb.com.
       *     -dryRun: (Optional) Does everything except do the actual file renaming.
       *     -collapseMultiPart: (Optional) Attempts to automatically collapse episodes specified as multipart into one local file.
       */

      private static string LocalDatabaseFileName = "localStore.db3";

      static void Main( string[] args )
      {
         CommandLine<CommandLineArguments> commandLine = null;

         try
         {
            commandLine = new CommandLine<CommandLineArguments>();
            var clArguments = commandLine.Parse( args );

            var seasonPathInfo = new SeasonPathInfo( clArguments.SeasonPath );

            IList<Episode> episodeList = null;

            var localStoragePath = new LocalStoragePath( Program.LocalDatabaseFileName );

            if ( clArguments.ForceUpdate )
            {
               Program.PurgeLocalData( localStoragePath );
            }
            else
            {
               episodeList = Program.GetEpisodesLocally( localStoragePath, clArguments, seasonPathInfo );
            }

            if ( ( episodeList == null ) || ( episodeList.Count == 0 ) )
            {
               episodeList = Program.GetEpisodesRemotely( clArguments, seasonPathInfo );

               if ( ( episodeList != null ) && ( episodeList.Count > 0 ) )
               {
                  Console.WriteLine( "Storing episode information locally." );
                  Program.StoreEpisodesLocally( localStoragePath, clArguments, seasonPathInfo, episodeList );
                  Console.WriteLine( "Successfully stored episode information locally." );
               }

               for ( var i = episodeList.Count - 1; i >= 0; i-- )
               {
                  if ( episodeList[i].SeasonNumber != seasonPathInfo.SeasonNumber )
                  {
                     episodeList.RemoveAt( i );
                  }
               }
            }

            if ( ( episodeList == null ) || ( episodeList.Count == 0 ) )
            {
               throw new NoEpisodesFoundException( String.Format( "No episodes found for \"{0}\" Season {1}.", seasonPathInfo.SeriesName, seasonPathInfo.SeasonNumber ) );
            }

            Console.WriteLine( String.Format( "Renaming local files in directory{0}:", clArguments.DryRun ? " (Dry Run)" : String.Empty ) );
            Console.WriteLine( String.Format( "  \"{0}\".", seasonPathInfo.SeasonPath ) );

            var fileRenamer = new FileRenamer();
            fileRenamer.FileRenamed += ( sender, e ) =>
               {
                  Console.WriteLine( String.Format( "    \"{0}\" -> \"{1}\"", e.OldName, e.NewName ) );
               };

            fileRenamer.RenameSeasonEpisodeFiles( seasonPathInfo.SeasonPath, episodeList, clArguments.DryRun, clArguments.CollapseMultiPart );

            Console.WriteLine( String.Format( "Successfully finished renaming local files{0}.", clArguments.DryRun ? " (Dry Run)" : String.Empty ) );
            Console.WriteLine();
         }
         catch ( MultipleSeriesReturnedException ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine();
            Console.WriteLine( String.Format( "Multiple series found for search term.  If no search term was supplied, try again with one.  If a search term was supplied, try again with a more specific one.  You can also specify a series ID with the \"-seriesId\" argument.  Use the list of matched series below to help." ) );
            Console.WriteLine();

            var seriesIdLabel = "Series ID";
            var maxName = 0;
            var maxId = seriesIdLabel.Length;
            foreach ( var series in ex.SeriesReturned )
            {
               if ( series.Name.Length > maxName )
               {
                  maxName = series.Name.Length;
               }
               var idLen = series.SeriesId.ToString().Length;
               if ( idLen > maxId )
               {
                  maxId = idLen;
               }
            }

            Console.WriteLine( String.Format( "  {0} {1}", "Series Name".PadRight( maxName ), seriesIdLabel ) );

            var lineBuilder = new StringBuilder( "  " );
            for ( var i = 0; i < maxName + maxId + 1; i++ )
            {
               lineBuilder.Append( '-' );
            }
            Console.WriteLine( lineBuilder.ToString() );

            foreach ( var series in ex.SeriesReturned )
            {
               Console.WriteLine( String.Format( "  {0} {1}", series.Name.PadRight( maxName ), series.SeriesId.ToString().PadLeft( maxId ) ) );
            }
            Console.WriteLine();
         }
         catch ( UnexpectedEpisodeCountException ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine();
            Console.WriteLine( ex.Message );
            Console.WriteLine( "Sometimes this will happen because thetvdb.com lists multiple part episodes separately, but the parts are combined into one file locally.  If this is the case, try using the -collapseMultiPart parameter." );
            Console.WriteLine();
         }
         catch ( CommandLineException ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine();
            Console.WriteLine( ex.Message );
            Console.WriteLine();
            Console.WriteLine( commandLine.Help() );
            Console.WriteLine();
         }
         catch ( Tvdb2FileException ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine();
            Console.WriteLine( ex.Message );
            Console.WriteLine();
         }
         catch ( Exception ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine();
            Console.WriteLine( String.Format( "Error: {0}", ex.Message ) );
            Console.WriteLine();
         }
      }

      private static IList<Episode> GetEpisodesLocally( LocalStoragePath localStoragePath, CommandLineArguments clArguments, SeasonPathInfo seasonPathInfo )
      {
         var episodeList = new List<Episode>();

         using ( var database = new SqliteDatabase() )
         {
            if ( localStoragePath.DoesActualPathExist() )
            {
               database.Open( localStoragePath );
               Console.WriteLine( String.Format( "Opened local storage file \"{0}\".", database.DatabasePath ) );
               database.BeginTransaction();

               if ( clArguments.SeriesId == CommandLineArguments.NoSeriesId )
               {
                  var searchTerms = ( !String.IsNullOrEmpty( clArguments.SeriesSearchTerms ) ) ? clArguments.SeriesSearchTerms : seasonPathInfo.SeriesName;
                  Console.WriteLine( String.Format( "Checking local storage for search term \"{0}\".", searchTerms ) );
                  var episodes = database.FindEpisodes( searchTerms, seasonPathInfo.SeasonNumber );

                  if ( episodes != null )
                  {
                     Console.WriteLine( "Loading episode information from local storage." );
                     episodeList.AddRange( episodes );
                  }
               }
               else
               {
                  Console.WriteLine( String.Format( "Checking local storage for series ID \"{0}\".", clArguments.SeriesId ) );
                  var episodes = database.FindEpisodes( clArguments.SeriesId, seasonPathInfo.SeasonNumber );
                  Console.WriteLine( "Loading episode information from local storage." );
                  episodeList.AddRange( episodes );
               }
            }
            else
            {
               database.Open( localStoragePath );
               Console.WriteLine( String.Format( "Opened local storage file \"{0}\".", database.DatabasePath ) );
               database.CreateTableSeries();
               database.CreateTableSeason();
               database.CreateTableEpisode();
            }

            database.EndTransaction();
         }

         return episodeList;
      }

      private static IList<Episode> GetEpisodesRemotely( CommandLineArguments clArguments, SeasonPathInfo seasonPathInfo )
      {
         var episodeList = new List<Episode>();

         var tvdbClient = new TvdbClient();
         tvdbClient.LookingUpSeries += ( sender, e ) =>
         {
            Console.WriteLine( "Looking up series on thetvdb.com." );
         };
         tvdbClient.LookUpSeriesComplete += ( sender, e ) =>
         {
            Console.WriteLine( "Successfully found series information." );
         };
         tvdbClient.DownloadingEpisodeInformation += ( sender, e ) =>
         {
            Console.WriteLine( "Downloading episode information." );
         };
         tvdbClient.DownloadingEpisodeInformationComplete += ( sender, e ) =>
         {
            Console.WriteLine( "Successfully finished downloading episode information." );
         };

         Stream episodeData = null;

         if ( clArguments.SeriesId == CommandLineArguments.NoSeriesId )
         {
            var searchTerms = ( !String.IsNullOrEmpty( clArguments.SeriesSearchTerms ) ) ? clArguments.SeriesSearchTerms : seasonPathInfo.SeriesName;
            episodeData = tvdbClient.GetSeriesEpisodeData( searchTerms, "en" );
         }
         else
         {
            episodeData = tvdbClient.GetSeriesEpisodeData( clArguments.SeriesId, "en" );
         }

         var seriesEpisodeParser = new SeriesEpisodeDataParser();
         var allEpisodeList = seriesEpisodeParser.ParseXmlData( episodeData );

         foreach ( var episode in allEpisodeList )
         {
            episodeList.Add( episode );
         }
         
         return episodeList;
      }

      private static void StoreEpisodesLocally( LocalStoragePath localStoragePath, CommandLineArguments clArguments, SeasonPathInfo seasonPathInfo, IList<Episode> episodeList )
      {
         var count = 0;

         using ( var database = new SqliteDatabase() )
         {
            database.Open( localStoragePath );
            Console.WriteLine( String.Format( "Opened local storage file \"{0}\".", database.DatabasePath ) );
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
                        Name = seasonPathInfo.SeriesName,
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

      private static void PurgeLocalData( LocalStoragePath localStoragePath )
      {
         using ( var database = new SqliteDatabase() )
         {
            database.Open( localStoragePath );
            database.DeleteAllEpisodes();
            database.DeleteAllSeasons();
            database.DeleteAllSeries();
         }
      }
   }
}
