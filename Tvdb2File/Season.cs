//////////////////////////////////////////////////////////////////////////////
// <copyright file="Season.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////

using System;

namespace Tvdb2File
{
   public class Season
   {
      public Int64 SeasonId
      {
         get;
         set;
      }

      public int SeasonNumber
      {
         get;
         set;
      }

      public Int64 SeriesId
      {
         get;
         set;
      }
   }
}
