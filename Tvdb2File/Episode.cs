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
         this.MultiPartNumber = 0;
      }

      public void SetName( string name, int multiPartNumber )
      {
         this.Name = name;
         this.NameBase = this.Name;
         this.MultiPartNumber = multiPartNumber;

         if ( this.MultiPartNumber == 0 )
         {
            this.MultiPartNumber = this.MultiPartFromName();
         }
      }

      private int MultiPartFromName()
      {
         var begin = this.Name.LastIndexOf( '(' );

         if ( begin < 0 )
         {
            return 0;
         }

         this.NameMultiPartStart = begin;

         begin++;

         var end = this.Name.LastIndexOf( ')' );

         if ( end < 0 )
         {
            return 0;
         }

         try
         {
            var number = Int32.Parse( this.Name.Substring( begin, end - begin ) );

            this.NameBase = this.Name.Substring( 0, begin - 2 );

            return number;
         }
         catch ( FormatException )
         {
            return 0;
         }
      }
   }
}
