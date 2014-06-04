//////////////////////////////////////////////////////////////////////////////
// <copyright file="CommandLine.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////
      
using System;
using System.IO;

namespace Tvdb2File
{
   public class CommandLine
   {
      private enum ArgumentState
      {
         Label,
         SeasonPath,
         SearchTerm,
         SeriesId
      }

      public const int NoSeriesId = -1;

      public string SeasonPath
      {
         get;
         private set;
      }

      public string SeriesSearchTerms
      {
         get;
         private set;
      }

      public int SeriesId
      {
         get;
         private set;
      }

      public CommandLine()
      {
         this.SeriesSearchTerms = String.Empty;
         this.SeriesId = CommandLine.NoSeriesId;
      }

      public static string Help()
      {
         return @"Command line usage:

Tvdb2File.exe -season ""path to season"" [[-search ""series search term""]|[-seriesId xxxxx]]
  -season: (Mandatory) Relative or absolute path to the directory containing the season to rename.
  -search: (Optional) Terms to use to search for the series.  If not supplied, the name of the series directory will be used.  This argument is mutually exclusive with -seriesId.
  -seriesId: (Optional) ID of the series to use for episode naming.  This argument is mutually exclusive with -search.";
      }

      public void Parse( string[] args )
      {
         var state = ArgumentState.Label;
         var seasonPathFound = false;
         var seriesSearchTermsFound = false;
         var seriesIdFound = false;

         for ( var i = 0; i < args.Length; i++ )
         {
            if ( state == ArgumentState.Label )
            {
               if ( String.Compare( args[i], "-season", true ) == 0 )
               {
                  seasonPathFound = true;
                  state = ArgumentState.SeasonPath;
               }
               else if ( String.Compare( args[i], "-search", true ) == 0 )
               {
                  if ( seriesIdFound )
                  {
                     throw new CommandLineException( "The argument -seriesId was already specified and is mutually exclusive with the -search argument." );
                  }

                  seriesSearchTermsFound = true;
                  state = ArgumentState.SearchTerm;
               }
               else if ( String.Compare( args[i], "-seriesId", true ) == 0 )
               {
                  if ( seriesSearchTermsFound )
                  {
                     throw new CommandLineException( "The argument -search was already specified and is mutually exclusive with the -seriesId argument." );
                  }

                  seriesIdFound = true;
                  state = ArgumentState.SeriesId;
               }
               else
               {
                  throw new CommandLineException( String.Format( "Unknown command line argument label \"{0}\".", args[i] ) );
               }
            }
            else if ( state == ArgumentState.SeasonPath )
            {
               if ( !Directory.Exists( args[i] ) )
               {
                  throw new CommandLineException( "The specified path to the season directory does not exist." );
               }

               this.SeasonPath = args[i];

               state = ArgumentState.Label;
            }
            else if ( state == ArgumentState.SearchTerm )
            {
               this.SeriesSearchTerms = args[i];

               state = ArgumentState.Label;
            }
            else if ( state == ArgumentState.SeriesId )
            {
               try
               {
                  this.SeriesId = Int32.Parse( args[i] );
               }
               catch ( FormatException ex )
               {
                  throw new CommandLineException( String.Format( "The specified series ID (\"{0}\") was not an integer.", args[i] ), ex );
               }

               state = ArgumentState.Label;
            }
            else
            {
               throw new CommandLineException( String.Format( "Unknown argument state \"{0}\".", state ) );
            }
         }

         if ( !seasonPathFound )
         {
            throw new CommandLineException( "Missing the path to the season (\"-season\")." );
         }
         else if ( String.IsNullOrEmpty( this.SeasonPath ) )
         {
            throw new CommandLineException( "The season path was not specified." );
         }
      }
   }
}
