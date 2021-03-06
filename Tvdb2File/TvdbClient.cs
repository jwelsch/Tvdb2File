﻿//////////////////////////////////////////////////////////////////////////////
// <copyright file="TvdbClient.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Ionic.Zip;

namespace Tvdb2File
{
   public class TvdbClient
   {
      private string apiKey = "FEBA0DC79E88C5F0";

      public event EventHandler LookingUpSeries;
      public event EventHandler LookUpSeriesComplete;
      public event EventHandler DownloadingEpisodeInformation;
      public event EventHandler DownloadingEpisodeInformationComplete;

      public Stream GetSeriesEpisodeData( string seriesName, string language )
      {
         if ( String.IsNullOrEmpty( language ) )
         {
            language = "en";
         }

         var seriesId = this.GetSeriesId( seriesName, language );
         return this.GetSeriesEpisodeData( seriesId, language );
      }

      public Stream GetSeriesEpisodeData( int seriesId, string language )
      {
         if ( String.IsNullOrEmpty( language ) )
         {
            language = "en";
         }

         var episodeData = this.DownloadEpisodeData( seriesId, language );

         return episodeData;
      }

      private string GetHttpStringData( Uri uri )
      {
         var httpRequest = HttpWebRequest.CreateHttp( uri );

         using ( var httpResponse = (HttpWebResponse) httpRequest.GetResponse() )
         {
            using ( var responseStream = httpResponse.GetResponseStream() )
            {
               using ( var streamReader = new StreamReader( responseStream ) )
               {
                  return streamReader.ReadToEnd();
               }
            }
         }
      }

      private void GetHttpBinaryData( Uri uri, Stream stream )
      {
         var httpRequest = HttpWebRequest.CreateHttp( uri );

         using ( var httpResponse = (HttpWebResponse) httpRequest.GetResponse() )
         {
            using ( var responseStream = httpResponse.GetResponseStream() )
            {
               var bufferSize = 256 * 1024; // 262,144 bytes = 1/4 MB
               var buffer = new byte[bufferSize];

               while ( true )
               {
                  var bytesRead = responseStream.Read( buffer, 0, bufferSize );

                  if ( bytesRead == 0 )
                  {
                     break;
                  }

                  stream.Write( buffer, 0, bytesRead );
               }
            }
         }
      }

      private int GetSeriesId( string seriesName, string language )
      {
         if ( this.LookingUpSeries != null )
         {
            this.LookingUpSeries( this, EventArgs.Empty );
         }

         var uriString = String.Format( @"http://thetvdb.com/api/GetSeries.php?seriesname={0}&language={1}", seriesName, language );
         var uri = new Uri( uriString );
         var seriesData = this.GetHttpStringData( uri );

         var xmlDoc = new XmlDocument();
         xmlDoc.LoadXml( seriesData );

         var seriesNodes = xmlDoc.SelectNodes( "Data/Series" );

         if ( seriesNodes.Count == 0 )
         {
            throw new NoSeriesFoundException( String.Format( "No series found that matches name \"{0}\".", seriesName ) );
         }
         else if ( seriesNodes.Count > 1 )
         {
            var seriesList = new List<Series>();

            foreach ( XmlNode seriesNode in seriesNodes )
            {
               var seriesNameNode = seriesNode.SelectSingleNode( "SeriesName" );
               var seriesIdNode = seriesNode.SelectSingleNode( "id" );

               seriesList.Add( new Series() { Name = seriesNameNode.InnerText, SeriesId = Int32.Parse( seriesIdNode.InnerText ) } );
            }

            throw new MultipleSeriesReturnedException( seriesList );
         }

         var node = xmlDoc.SelectSingleNode( "Data/Series/id" );

         if ( this.LookUpSeriesComplete != null )
         {
            this.LookUpSeriesComplete( this, EventArgs.Empty );
         }

         try
         {
            return Int32.Parse( node.InnerText );
         }
         catch ( FormatException ex )
         {
            throw new XmlFormatException( String.Format( "The series ID (\"{0}\") did not evaluate to an integer.", node.InnerText ), ex );
         }
      }

      private Stream DownloadEpisodeData( int seriesId, string language )
      {
         if ( this.DownloadingEpisodeInformation != null )
         {
            this.DownloadingEpisodeInformation( this, EventArgs.Empty );
         }

         var uriString = String.Format( @"http://thetvdb.com/api/{0}/series/{1}/all/{2}.zip", this.apiKey, seriesId, language );

         var episodeXml = new MemoryStream();

         // TODO: Uncomment after testing!!
         //using ( var zipData = new FileStream( "C:\\Temp\\xml.zip", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read ) )
         using ( var zipData = new MemoryStream() )
         {
            try
            {
               this.GetHttpBinaryData( new Uri( uriString ), zipData );
            }
            catch ( WebException ex )
            {
               if ( ( (HttpWebResponse) ex.Response ).StatusCode == HttpStatusCode.NotFound )
               {
                  throw new NoSeriesFoundException( String.Format( "No series with ID \"{0}\" could be found.", seriesId ), ex );
               }
               else
               {
                  throw ex;
               }
            }

            zipData.Seek( 0, SeekOrigin.Begin );

            using ( var zip = ZipFile.Read( zipData ) )
            {
               foreach ( var entry in zip )
               {
                  if ( String.Compare( entry.FileName, String.Format( "{0}.xml", language ) ) == 0 )
                  {
                     entry.Extract( episodeXml );
                     episodeXml.Seek( 0, SeekOrigin.Begin );
                     break;
                  }
               }
            }
         }

         if ( this.DownloadingEpisodeInformationComplete != null )
         {
            this.DownloadingEpisodeInformationComplete( this, EventArgs.Empty );
         }

         return episodeXml;
      }
   }
}
