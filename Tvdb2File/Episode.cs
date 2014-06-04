//////////////////////////////////////////////////////////////////////////////
// <copyright file="Episode.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////

using System;

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
   }
}
