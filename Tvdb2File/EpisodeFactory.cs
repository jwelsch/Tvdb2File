//////////////////////////////////////////////////////////////////////////////
// <copyright file="EpisodeFactory.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////
      

using System;
using System.Xml;

namespace Tvdb2File
{
   public class EpisodeFactory
   {
      public Episode Create( XmlReader xmlReader )
      {
         var allRead = 127U;
         var infoRead = 0U;
         var episode = new Episode();

         while ( xmlReader.Read() && ( infoRead != allRead ) )
         {
            if ( XmlHelper.CheckElement( xmlReader, "id", XmlNodeType.Element, false ) )
            {
               xmlReader.Read();
               episode.EpisodeId = Int32.Parse( xmlReader.Value );
               infoRead |= 1U;
            }
            else if ( XmlHelper.CheckElement( xmlReader, "EpisodeName", XmlNodeType.Element, false ) )
            {
               xmlReader.Read();
               episode.Name = xmlReader.Value;
               infoRead |= 2U;
            }
            else if ( XmlHelper.CheckElement( xmlReader, "EpisodeNumber", XmlNodeType.Element, false ) )
            {
               xmlReader.Read();
               episode.EpisodeNumber = Int32.Parse( xmlReader.Value );
               infoRead |= 4U;
            }
            else if ( XmlHelper.CheckElement( xmlReader, "SeasonNumber", XmlNodeType.Element, false ) )
            {
               xmlReader.Read();
               episode.SeasonNumber = Int32.Parse( xmlReader.Value );
               infoRead |= 8U;
            }
            else if ( XmlHelper.CheckElement( xmlReader, "seasonid", XmlNodeType.Element, false ) )
            {
               xmlReader.Read();
               episode.SeasonId = Int32.Parse( xmlReader.Value );
               infoRead |= 16U;
            }
            else if ( XmlHelper.CheckElement( xmlReader, "seriesid", XmlNodeType.Element, false ) )
            {
               xmlReader.Read();
               episode.SeriesId = Int32.Parse( xmlReader.Value );
               infoRead |= 32U;
            }
            else if ( XmlHelper.CheckElement( xmlReader, "Language", XmlNodeType.Element, false ) )
            {
               xmlReader.Read();
               episode.Language = xmlReader.Value;
               infoRead |= 64U;
            }
         }

         if ( infoRead != allRead )
         {
            var missing = string.Empty;

            if ( !infoRead.BitFlagSet( 1U ) )
            {
               missing = "id";
            }
            else if ( !infoRead.BitFlagSet( 2U ) )
            {
               missing = "EpisodeName";
            }
            else if ( !infoRead.BitFlagSet( 4U ) )
            {
               missing = "EpisodeNumber";
            }
            else if ( !infoRead.BitFlagSet( 8U ) )
            {
               missing = "SeasonNumber";
            }
            else if ( !infoRead.BitFlagSet( 16U ) )
            {
               missing = "seasonid";
            }
            else if ( !infoRead.BitFlagSet( 32U ) )
            {
               missing = "seriesid";
            }
            else if ( !infoRead.BitFlagSet( 64U ) )
            {
               missing = "Language";
            }

            throw new XmlFormatException( String.Format( "Missing element \"{0}\" from episode XML.", missing ) );
         }

         return episode;
      }
   }
}
