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
         var commandText =
@"CREATE TABLE IF NOT EXISTS Series
(
Id INTEGER UNIQUE PRIMARY KEY NOT NULL
, Name TEXT NOT NULL
)";

         using ( var command = new SQLiteCommand( commandText, this.connection, this.transaction ) )
         {
            command.ExecuteNonQuery();
         }
      }

      public void CreateTableSeason()
      {
         var commandText =
@"CREATE TABLE IF NOT EXISTS Season
(
Id INTEGER UNIQUE PRIMARY KEY NOT NULL
, Number INT NOT NULL
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
         var commandText =
@"CREATE TABLE IF NOT EXISTS Episode
(
Id INTEGER UNIQUE PRIMARY KEY NOT NULL
, Name TEXT NOT NULL
, Number INT NOT NULL
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

      public Series FindSeries( string seriesSearch )
      {
         var commandText =
@"SELECT sn.Id, sn.Name
FROM Series AS sn
WHERE
   sn.Name LIKE '$seriesName';";

         var seriesList = new List<Series>();

         using ( var command = new SQLiteCommand( commandText, this.connection, this.transaction ) )
         {
            command.Parameters.AddWithValue( "$seriesName", seriesSearch );

            using ( var reader = command.ExecuteReader() )
            {
               while ( reader.Read() )
               {
                  var series = new Series();
                  series.SeriesId = (Int64) reader["Id"];
                  series.Name = (string) reader["Name"];
                  seriesList.Add( series );
               }
            }
         }

         if ( seriesList.Count == 0 )
         {
            throw new NoSeriesFoundException( String.Format( "No series found that matches name \"{0}\".", seriesSearch ) );
         }
         else if ( seriesList.Count > 1 )
         {
            throw new MultipleSeriesReturnedException( seriesList );
         }

         return seriesList[0];
      }

      public Series FindSeries( Int64 seriesId )
      {
         var commandText =
@"SELECT sn.Id, sn.Name
FROM Series AS sn
WHERE
   sn.Id = $seriesId;";

         Series series = null;

         using ( var command = new SQLiteCommand( commandText, this.connection, this.transaction ) )
         {
            command.Parameters.AddWithValue( "$seriesId", seriesId );

            using ( var reader = command.ExecuteReader() )
            {
               if ( reader.Read() )
               {
                  series = new Series();
                  series.SeriesId = (Int64) reader["Id"];
                  series.Name = (string) reader["Name"];
               }
            }
         }

         return series;
      }

      public Episode FindEpisode( Int64 episodeId )
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
                  episode.EpisodeId = (Int64) reader["ep.Id"];
                  episode.Name = (string) reader["ep.Name"];
                  episode.EpisodeNumber = (int) reader["ep.Number"];
                  episode.SeasonNumber = (int) reader["sn.Number"];
                  episode.SeriesId = (Int64) reader["ep.SeriesId"];
                  episode.SeasonId = (Int64) reader["ep.SeasonId"];
               }
            }
         }

         return episode;
      }

      public Episode FindEpisode( Int64 seriesId, int seasonNumber, int episodeNumber )
      {
         Episode episode = null;
         var commandText =
@"SELECT ep.Id, ep.Name, ep.Number, ep.Language, sn.Number, ep.SeriesId, ep.SeasonId
FROM Episode AS ep
LEFT JOIN Season AS sn ON ep.SeasonId = sn.Id
WHERE
   ep.SeriesId = $seriesId
   AND sn.Number = $seasonNumber
   AND ep.Number = $episodeNumber;";

         using ( var command = new SQLiteCommand( commandText, this.connection, this.transaction ) )
         {
            command.Parameters.AddWithValue( "$seriesId", seriesId );
            command.Parameters.AddWithValue( "$seasonNumber", seasonNumber );
            command.Parameters.AddWithValue( "$episodeNumber", episodeNumber );

            using ( var reader = command.ExecuteReader() )
            {
               if ( reader.Read() )
               {
                  episode = new Episode()
                  {
                     EpisodeId = (Int64) reader.GetValue( 0 ),
                     Name = (string) reader.GetValue( 1 ),
                     EpisodeNumber = (int) reader.GetValue( 2 ),
                     Language = (string) reader.GetValue( 3 ),
                     SeasonNumber = (int) reader.GetValue( 4 ),
                     SeriesId = (Int64) reader.GetValue( 5 ),
                     SeasonId = (Int64) reader.GetValue( 6 )
                  };
               }
            }
         }

         return episode;
      }

      public IList<Episode> FindEpisodes( Int64 seriesId, int seasonNumber )
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
                  //for ( var i = 0; i < reader.FieldCount; i++ )
                  //{
                  //   var name = reader.GetName( i );
                  //   var type = reader.GetFieldType( i );
                  //}
                  episode = new Episode()
                  {
                     EpisodeId = (Int64) reader.GetValue( 0 ),
                     Name = (string) reader.GetValue( 1 ),
                     EpisodeNumber = (int) reader.GetValue( 2 ),
                     Language = (string) reader.GetValue( 3 ),
                     SeasonNumber = (int) reader.GetValue( 4 ),
                     SeriesId = (Int64) reader.GetValue( 5 ),
                     SeasonId = (Int64) reader.GetValue( 6 )
                  };
                  episodes.Add( episode );
               }
            }
         }

         return episodes;
      }

      public IList<Episode> FindEpisodes( string seriesSearch, int seasonNumber )
      {
         var series = this.FindSeries( seriesSearch );

         return this.FindEpisodes( series.SeriesId, seasonNumber );
      }

      public Season FindSeason( Int64 seriesId, Int64 seasonId )
      {
         var commandText =
@"SELECT sn.Id, sn.Number, sn.SeriesId
FROM Season AS sn
WHERE
   sn.Id = $seasonId
   AND sn.SeriesId = $seriesId";

         Season season = null;

         using ( var command = new SQLiteCommand( commandText, this.connection, this.transaction ) )
         {
            command.Parameters.AddWithValue( "$seasonId", seasonId );
            command.Parameters.AddWithValue( "$seriesId", seriesId );

            using ( var reader = command.ExecuteReader() )
            {
               if ( reader.Read() )
               {
                  season = new Season()
                  {
                     SeasonId = (Int64) reader["Id"],
                     SeasonNumber = (int) reader["Number"],
                     SeriesId = (Int64) reader["SeriesId"]
                  };
               }
            }
         }

         return season;
      }

      public void InsertSeries( Series series )
      {
         var commandText =
@"INSERT INTO Series
( Id, Name )
VALUES ( $id, $name );";

         using ( var command = new SQLiteCommand( commandText, this.connection, this.transaction ) )
         {
            command.Parameters.AddWithValue( "$id", series.SeriesId );
            command.Parameters.AddWithValue( "$name", series.Name );

            command.ExecuteNonQuery();
         }
      }

      public void InsertSeason( Season season )
      {
         var commandText =
@"INSERT INTO Season
( Id, Number, SeriesId )
VALUES ( $id, $seasonNumber, $seriesId );";

         using ( var command = new SQLiteCommand( commandText, this.connection, this.transaction ) )
         {
            command.Parameters.AddWithValue( "$id", season.SeasonId );
            command.Parameters.AddWithValue( "$seasonNumber", season.SeasonNumber );
            command.Parameters.AddWithValue( "$seriesId", season.SeriesId );

            command.ExecuteNonQuery();
         }
      }

      public void InsertEpisode( Episode episode )
      {
         var commandText =
@"INSERT INTO Episode
( Id, Name, Number, SeriesId, SeasonId, Language )
VALUES ( $id, $name, $number, $seriesId, $seasonId, $language );";

         using ( var command = new SQLiteCommand( commandText, this.connection, this.transaction ) )
         {
            command.Parameters.AddWithValue( "$id", episode.EpisodeId );
            command.Parameters.AddWithValue( "$name", episode.Name );
            command.Parameters.AddWithValue( "$number", episode.EpisodeNumber );
            command.Parameters.AddWithValue( "$seriesId", episode.SeriesId );
            command.Parameters.AddWithValue( "$seasonId", episode.SeasonId );
            command.Parameters.AddWithValue( "$language", episode.Language );

            command.ExecuteNonQuery();
         }
      }
   }
}
