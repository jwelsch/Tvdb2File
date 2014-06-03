//////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
       *   Tvdb2File.exe -season "path to season" [[-search "series search term"]|[-seriesId xxxxx]]
       *     -season: (Mandatory) Relative or absolute path to the directory containing the season to rename.
       *     -search: (Optional) Terms to use to search for the series.  If not supplied, the name of the series directory will be used.  This argument is mutually exclusive with -seriesId.
       *     -seriesId: (Optional) ID of the series to use for episode naming.  This argument is mutually exclusive with -search.
       */

      private const string LocalDatabasePath = "localStore.db3";

      static void Main( string[] args )
      {
         try
         {
            var commandLine = new CommandLine();
            commandLine.Parse( args );

            var seasonPathInfo = new SeasonPathInfo( commandLine.SeasonPath );

            var episodeList = Program.GetEpisodesLocally( commandLine, seasonPathInfo );

            if ( ( episodeList == null ) || ( episodeList.Length == 0 ) )
            {
               episodeList = Program.GetEpisodesRemotely( commandLine, seasonPathInfo );

               if ( ( episodeList != null ) && ( episodeList.Length > 0 ) )
               {
                  Program.StoreEpisodesLocally( commandLine, seasonPathInfo, episodeList );
               }
            }

            if ( ( episodeList == null ) || ( episodeList.Length == 0 ) )
            {
               throw new NoEpisodesFoundException( String.Format( "No episodes found for \"{0}\" Season {1}.", seasonPathInfo.SeriesName, seasonPathInfo.SeasonNumber ) );
            }

            Console.WriteLine( "Renaming local files in directory:" );
            Console.WriteLine( String.Format( "  \"{0}\".", seasonPathInfo.SeasonPath ) );

            var fileRenamer = new FileRenamer();
            fileRenamer.FileRenamed += ( sender, e ) =>
               {
                  Console.WriteLine( String.Format( "    \"{0}\" -> \"{1}\"", e.OldName, e.NewName ) );
               };

            fileRenamer.RenameSeasonEpisodeFiles( seasonPathInfo.SeasonPath, episodeList );

            Console.WriteLine( "Successfully finished renaming local files." );
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
            Console.WriteLine( "Sometimes this will happen because thetvdb.com lists multiple part episodes separately, but the parts are combined into one file locally.  If this is the case, simply create a dummy file and name it such that it is immediately after the combined file.  Tvdb2File will rename the combined file with a \"Part X\" and the dummy file with a \"Part Y\".  You can then remove the \"Part X\" from the combined file and delete the dummy file manually." );
            Console.WriteLine();
         }
         catch ( CommandLineException ex )
         {
            System.Diagnostics.Trace.WriteLine( ex );
            Console.WriteLine();
            Console.WriteLine( ex.Message );
            Console.WriteLine( CommandLine.Help() );
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

      private static Episode[] GetEpisodesLocally( CommandLine commandLine, SeasonPathInfo seasonPathInfo )
      {
         var episodeList = new List<Episode>();

         using ( var database = new SqliteDatabase() )
         {
            if ( File.Exists( Program.LocalDatabasePath ) )
            {
               database.Open( Program.LocalDatabasePath );

               if ( commandLine.SeriesId == CommandLine.NoSeriesId )
               {
                  var searchTerms = ( !String.IsNullOrEmpty( commandLine.SeriesSearchTerms ) ) ? commandLine.SeriesSearchTerms : seasonPathInfo.SeriesName;
                  episodeList.AddRange( database.FindEpisodes( searchTerms, seasonPathInfo.SeasonNumber ) );
               }
               else
               {
                  episodeList.AddRange( database.FindEpisodes( commandLine.SeriesId, seasonPathInfo.SeasonNumber ) );
               }
            }
            else
            {
               database.Open( Program.LocalDatabasePath );
               database.CreateTableSeries();
               database.CreateTableSeason();
               database.CreateTableEpisode();
            }
         }

         return episodeList.ToArray();
      }

      private static Episode[] GetEpisodesRemotely( CommandLine commandLine, SeasonPathInfo seasonPathInfo )
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

         if ( commandLine.SeriesId == CommandLine.NoSeriesId )
         {
            var searchTerms = ( !String.IsNullOrEmpty( commandLine.SeriesSearchTerms ) ) ? commandLine.SeriesSearchTerms : seasonPathInfo.SeriesName;
            episodeData = tvdbClient.GetSeriesEpisodeData( searchTerms, "en" );
         }
         else
         {
            episodeData = tvdbClient.GetSeriesEpisodeData( commandLine.SeriesId, "en" );
         }

         var seriesEpisodeParser = new SeriesEpisodeDataParser();
         var allEpisodeList = seriesEpisodeParser.ParseXmlData( episodeData );

         foreach ( var episode in allEpisodeList )
         {
            if ( episode.SeasonNumber == seasonPathInfo.SeasonNumber )
            {
               episodeList.Add( episode );
            }
         }
         
         return episodeList.ToArray();
      }

      private static void StoreEpisodesLocally( CommandLine commandLine, SeasonPathInfo seasonPathInfo, IList<Episode> episodeList )
      {
         using ( var database = new SqliteDatabase() )
         {
            database.Open( Program.LocalDatabasePath );
         }
      }
   }
}
