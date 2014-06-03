//////////////////////////////////////////////////////////////////////////////
// <copyright file="SqliteDatabase.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace Tvdb2File
{
   public class SqliteDatabase : IDisposable
   {
      private SQLiteConnection connection;
      private SQLiteTransaction transaction;

      public SqliteDatabase()
      {
      }

      public void Open( string sqlitePath )
      {
         this.connection = new SQLiteConnection( String.Format( "Data Source=\"{0}\";", sqlitePath ) );
         this.connection.Open();
      }

      public void Close()
      {
         this.connection.Dispose();
      }

      #region IDisposable Members

      /// <summary>
      /// Implements the IDisposable.Dispose() method.
      /// Provides an opportunity to execute clean up code.
      /// </summary>
      public void Dispose()
      {
         // Clean up both managed and unmanaged resources.
         this.Dispose( true );

         // Prevent the garbage collector from calling Object.Finalize since
         // the clean up has already been done by the call to Dispose() above.
         GC.SuppressFinalize( this );
      }

      /// <summary>
      /// Releases managed and optionally unmanaged resources.
      /// </summary>
      /// <param name="disposing">Pass true to release both managed and unmanaged resources,
      /// false to release only unmanaged resources.</param>
      protected virtual void Dispose( bool disposing )
      {
         if ( disposing )
         {
            // TODO: Release managed resources here.

            if ( this.connection.State != System.Data.ConnectionState.Closed )
            {
               this.connection.Close();
            }
         }

         // TODO: Release unmanaged (native) resources here.
      }

      #endregion

      public void BeginTransaction()
      {
         this.transaction = this.connection.BeginTransaction();
      }

      public void EndTransaction()
      {
         if ( this.transaction != null )
         {
            this.transaction.Commit();
            this.transaction = null;
         }
      }

      public void CreateTableSeries()
      {
         var commandText = @"CREATE TABLE IF NOT EXISTS Series (
Id INTEGER PRIMARY KEY NOT NULL
, Name TEXT NOT NULL
)";

         using ( var command = new SQLiteCommand( commandText, this.connection, this.transaction ) )
         {
            command.ExecuteNonQuery();
         }
      }

      public void CreateTableSeason()
      {
         var commandText = @"CREATE TABLE IF NOT EXISTS Season (
Id INTEGER PRIMARY KEY NOT NULL
, Name TEXT NOT NULL
, Number INTEGER NOT NULL
, SeriesId INTEGER NOT NULL
, FOREIGN KEY( SeriesId ) REFERENCES Series( Id )
)";

         using ( var command = new SQLiteCommand( commandText, this.connection, this.transaction ) )
         {
            command.ExecuteNonQuery();
         }
      }

      public void CreateTableEpisode()
      {
         var commandText = @"CREATE TABLE IF NOT EXISTS Episode (
Id INTEGER PRIMARY KEY NOT NULL
, Name TEXT NOT NULL
, Number INTEGER NOT NULL
, SeriesId INTEGER NOT NULL
, SeasonId INTEGER NOT NULL
, Language TEXT NOT NULL
, FOREIGN KEY( SeriesId ) REFERENCES Series( Id )
, FOREIGN KEY( SeasonId ) REFERENCES Season( Id )
)";

         using ( var command = new SQLiteCommand( commandText, this.connection, this.transaction ) )
         {
            command.ExecuteNonQuery();
         }
      }

      public void DropTable( string tableName )
      {
         var commandText = new StringBuilder();
         commandText.AppendFormat( "DROP TABLE {0}", tableName );
         using ( var command = new SQLiteCommand( commandText.ToString(), this.connection, this.transaction ) )
         {
            command.ExecuteNonQuery();
         }
      }

      public Episode FindEpisode( int episodeId )
      {
         Episode episode = null;
         var commandText =
@"SELECT ep.Id, ep.Name, ep.Number, ep.Language, sn.Number, ep.SeriesId, ep.SeasonId
FROM Episode AS ep
LEFT JOIN Season AS sn ON ep.SeasonId = sn.Id
WHERE
   ep.Id = $episodeId;";

         using ( var command = new SQLiteCommand( commandText, this.connection, this.transaction ) )
         {
            command.Parameters.AddWithValue( "$episodeId", episodeId );

            using ( var reader = command.ExecuteReader() )
            {
               if ( reader.Read() )
               {
                  episode = new Episode();
                  episode.EpisodeId = (int) reader["ep.Id"];
                  episode.Name = (string) reader["ep.Name"];
                  episode.EpisodeNumber = (int) reader["ep.Number"];
                  episode.SeasonNumber = (int) reader["sn.Number"];
                  episode.SeriesId = (int) reader["ep.SeriesId"];
                  episode.SeasonId = (int) reader["ep.SeasonId"];
               }
            }
         }

         return episode;
      }

      public Episode[] FindEpisodes( int seriesId, int seasonNumber )
      {
         var episodes = new List<Episode>();
         var commandText =
@"SELECT ep.Id, ep.Name, ep.Number, ep.Language, sn.Number, ep.SeriesId, ep.SeasonId
FROM Episode AS ep
LEFT JOIN Season AS sn ON ep.SeasonId = sn.Id
WHERE
   ep.SeriesId = $seriesId
   AND sn.Number = $seasonNumber;";

         using ( var command = new SQLiteCommand( commandText, this.connection, this.transaction ) )
         {
            command.Parameters.AddWithValue( "$seriesId", seriesId );
            command.Parameters.AddWithValue( "$seasonNumber", seasonNumber );

            using ( var reader = command.ExecuteReader() )
            {
               while ( reader.Read() )
               {
                  var episode = new Episode();
                  episode.EpisodeId = (int) reader["ep.Id"];
                  episode.Name = (string) reader["ep.Name"];
                  episode.EpisodeNumber = (int) reader["ep.Number"];
                  episode.SeasonNumber = (int) reader["sn.Number"];
                  episode.SeriesId = (int) reader["ep.SeriesId"];
                  episode.SeasonId = (int) reader["ep.SeasonId"];
                  episodes.Add( episode );
               }
            }
         }

         return episodes.ToArray();
      }
   }
}
