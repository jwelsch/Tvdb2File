//////////////////////////////////////////////////////////////////////////////
// <copyright file="SqliteClient.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace Tvdb2File
{
   public class SqliteClient
   {
      private SqliteDatabase database = new SqliteDatabase();

      public SqliteClient()
      {
      }

      public void Open( string dbPath )
      {
         this.database.Open( dbPath );
      }

      public void Close()
      {
         this.database.Close();
      }

      public IList<Episode> GetEpisodes( int seriesId, int seasonNumber )
      {
         var episodes = this.database.FindEpisodes( seriesId, seasonNumber );

         return episodes;
      }
   }
}
