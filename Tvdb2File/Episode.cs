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
         set;
      }

      public string Language
      {
         get;
         set;
      }

      public bool IsMultiPart
      {
         get { return this.MultiPartNumber != 0; }
      }

      public int MultiPartId
      {
         get;
         set;
      }

      public int MultiPartNumber
      {
         get;
         set;
      }

      public string FileName
      {
         get;
         set;
      }

      public Episode()
      {
         this.MultiPartNumber = 1;
      }

      public bool NameIsMultiPart()
      {
         var regex = new Regex( @".+? \(\d+\)$" );

         return regex.IsMatch( this.Name );
      }

      public int MultiPartFromName()
      {
         var openParen = this.Name.LastIndexOf( '(' );

         if ( openParen < 0 )
         {
            throw new ArgumentException( "Name does not contain multipart information." );
         }

         var closeParen = this.Name.LastIndexOf( ')' );

         if ( closeParen < 0 )
         {
            throw new ArgumentException( "Name does not contain multipart information." );
         }

         return Int32.Parse( this.Name.Substring( openParen + 1, closeParen - ( openParen + 1 ) ) );
      }
   }
}
