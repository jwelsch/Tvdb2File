//////////////////////////////////////////////////////////////////////////////
// <copyright file="Episode.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Text.RegularExpressions;

namespace Tvdb2File
{
   public class Episode
   {
      public Int64 EpisodeId
      {
         get;
         set;
      }

      public Int64 SeasonId
      {
         get;
         set;
      }

      public Int64 SeriesId
      {
         get;
         set;
      }

      public int SeasonNumber
      {
         get;
         set;
      }

      public int EpisodeNumber
      {
         get;
         set;
      }

      public string Name
      {
         get;
         private set;
      }

      public string NameBase
      {
         get;
         private set;
      }

      public string Language
      {
         get;
         set;
      }

      public bool IsMultiPart
      {
         get { return this.MultiPartNumber > 0; }
      }

      public int MultiPartId
      {
         get;
         set;
      }

      public int MultiPartNumber
      {
         get;
         private set;
      }

      public int NameMultiPartStart
      {
         get;
         private set;
      }

      public string FileName
      {
         get;
         set;
      }

      public Episode()
      {
      }

      public void SetName( string name, int multiPartNumber )
      {
         this.Name = name;
         this.NameBase = this.Name;
         this.MultiPartNumber = multiPartNumber;

         var begin = this.Name.LastIndexOf( '(' );

         if ( begin > 0 )
         {
            begin++;

            var end = this.Name.LastIndexOf( ')' );

            if ( end > 0 )
            {
               try
               {
                  var number = Int32.Parse( this.Name.Substring( begin, end - begin ) );

                  this.NameMultiPartStart = begin - 1;
                  this.NameBase = this.Name.Substring( 0, begin - 2 );

                  if ( this.MultiPartNumber == 0 )
                  {
                     this.MultiPartNumber = number;
                  }
                  else if ( this.MultiPartNumber != number )
                  {
                     // Sometimes thetvdb.com XML data is wrong - err on the side of the show name.
                     this.MultiPartNumber = number;
                     //throw new ArgumentException( String.Format( "The specified multipart episode number ({0}) is not the same as what is found in the episode name ({1}) of episode \"{2}\".", multiPartNumber, number, name ) );
                  }
               }
               catch ( FormatException )
               {
               }
            }
         }
      }

      public static string StripMultiPartSuffix( string name )
      {
         var regexParen = new Regex( @" \(\d+\)$" );
         var regexPart = new Regex( @" Part \d+$" );

         var match = regexParen.Match( name );
         if ( !match.Success )
         {
            match = regexPart.Match( name );
         }

         if ( match.Success )
         {
            return name.Substring( 0, match.Index );
         }

         return name;
      }
   }
}
