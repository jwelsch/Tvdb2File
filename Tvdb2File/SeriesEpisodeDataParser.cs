using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Tvdb2File
{
   public class SeriesEpisodeDataParser
   {
      public IList<Episode> ParseXmlData( Stream xmlData )
      {
         var episodeList = new List<Episode>();

         using ( var xmlReader = XmlReader.Create( xmlData, new XmlReaderSettings() { IgnoreComments = true, IgnoreWhitespace = true } ) )
         {
            XmlHelper.CheckNextElement( xmlReader, "xml", XmlNodeType.XmlDeclaration );
            XmlHelper.CheckNextElement( xmlReader, "Data", XmlNodeType.Element );
            XmlHelper.CheckNextElement( xmlReader, "Series", XmlNodeType.Element );
            xmlReader.Skip();

            var episodeFactory = new EpisodeFactory();

            do
            {
               if ( XmlHelper.CheckElement( xmlReader, "Episode", XmlNodeType.Element, false ) )
               {
                  var episode = episodeFactory.Create( xmlReader );
                  episodeList.Add( episode );
               }
            }
            while ( !XmlHelper.CheckNextElement( xmlReader, "Data", XmlNodeType.EndElement, false ) ) ;
         }

         return episodeList;
      }
   }
}
