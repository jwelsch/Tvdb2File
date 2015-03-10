//////////////////////////////////////////////////////////////////////////////
// <copyright file="SeasonPathInfo.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Tvdb2File
{
   public class SeasonPathInfo
   {
      public string SeasonPath
      {
         get;
         private set;
      }

      public int SeasonNumber
      {
         get;
         private set;
      }

      public string SeriesPath
      {
         get;
         private set;
      }

      public string SeriesName
      {
         get;
         private set;
      }

      public SeasonPathInfo( string seasonPath )
      {
         this.SeasonPath = seasonPath;
         this.SeriesPath = Path.GetDirectoryName( this.SeasonPath );

         if ( this.SeriesPath.Length <= 0 )
         {
            throw new ArgumentException( "No series directory specified." );
         }

         var slash = this.SeriesPath.LastIndexOf( '\\' );

         if ( slash < 0 )
         {
            this.SeriesName = this.SeriesPath.TrimEnd( '\\' );
         }
         else
         {
            this.SeriesName = this.SeriesPath.Substring( slash + 1 ).TrimEnd( '\\' );
         }

         var regex = new Regex( @"(?i)Season \d+$" );
         var match = regex.Match( this.SeasonPath );

         if ( !match.Success )
         {
            throw new ArgumentException( "The season directory in the path was not formatted correctly.  It needs to be of the form \"Season n\", where n is an integer." );
         }

         regex = new Regex( @"\d+" );
         match = regex.Match( match.Value );

         this.SeasonNumber = Int32.Parse( match.Value );
      }
   }
}
