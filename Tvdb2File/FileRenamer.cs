//////////////////////////////////////////////////////////////////////////////
// <copyright file="FileRenamer.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Tvdb2File
{
   #region FileRenamedArgs

   public class FileRenamedArgs : EventArgs
   {
      public string OldName
      {
         get;
         private set;
      }

      public string NewName
      {
         get;
         private set;
      }

      public FileRenamedArgs( string oldName, string newName )
      {
         this.OldName = oldName;
         this.NewName = newName;
      }
   }

   #endregion

   public class FileRenamer
   {
      public event EventHandler<FileRenamedArgs> FileRenamed;

      public int RenameSeasonEpisodeFiles( string seasonDirectoryPath, IList<Episode> episodeList, bool dryRun, bool collapseMultiPart )
      {
         var episodeFileNames = Directory.GetFiles( seasonDirectoryPath, "*.mp4", SearchOption.TopDirectoryOnly );
         var excessFiles = 0;

         if ( episodeFileNames.Length < episodeList.Count )
         {
            if ( collapseMultiPart )
            {
               excessFiles = episodeList.Count - episodeFileNames.Length;
            }
            else
            {
               throw new UnexpectedEpisodeCountException( String.Format( "The number of episode files ({0}) does not match the number of episodes from thetvdb.com ({1}).", episodeFileNames.Length, episodeList.Count ) );
            }
         }
         else if ( episodeFileNames.Length > episodeList.Count )
         {
            throw new UnexpectedEpisodeCountException( String.Format( "The number of episode files ({0}) does not match the number of episodes from thetvdb.com ({1}).", episodeFileNames.Length, episodeList.Count ) );
         }

         episodeList.Sort( ( item1, item2 ) =>
            {
               return item1.EpisodeNumber - item2.EpisodeNumber;
            } );
         this.CreateNewFileNameList( episodeList );
         this.RemovePartSuffixFromDissimilarEpisodeNames( episodeList );

         Array.Sort<string>( episodeFileNames, new NaturalStringComparer() );

         var episodeListIndex = 0;
         for ( var episodeFileNamesIndex = 0; episodeFileNamesIndex < episodeFileNames.Length; episodeFileNamesIndex++, episodeListIndex++ )
         {
            var fileName = episodeList[episodeListIndex].FileName;

            if ( episodeList[episodeListIndex].IsMultiPart && ( excessFiles > 0 ) )
            {
               fileName = this.MakeMultiPartFileName( episodeFileNamesIndex, episodeList );

               excessFiles--;
               episodeListIndex++;
            }

            var targetFileNameWithExt = fileName + Path.GetExtension( episodeFileNames[episodeFileNamesIndex] );
            var targetFilePath = Path.Combine( Path.GetDirectoryName( episodeFileNames[episodeFileNamesIndex] ), targetFileNameWithExt );

            if ( !dryRun )
            {
               // TODO: Uncomment when done testing!!
               File.Move( episodeFileNames[episodeFileNamesIndex], targetFilePath );
            }

            if ( this.FileRenamed != null )
            {
               this.FileRenamed( this, new FileRenamedArgs( Path.GetFileName( episodeFileNames[episodeFileNamesIndex] ), targetFileNameWithExt ) );
            }
         }

         return episodeFileNames.Length;
      }

      private void CreateNewFileNameList( IList<Episode> episodeList )
      {
         var newFileNameList = new List<string>();

         for ( var i = 0; i < episodeList.Count; i++ )
         {
            var normalizedName = this.NormalizeEpisodeName( episodeList[i] );
            episodeList[i].FileName = String.Format( "S{0:D2}E{1:D2} {2}", episodeList[i].SeasonNumber, episodeList[i].EpisodeNumber, normalizedName );
         }
      }

      private string NormalizeEpisodeName( Episode episode )
      {
         var normalizedName = new StringBuilder( episode.Name );

         if ( episode.IsMultiPart )
         {
            normalizedName.Remove( episode.NameMultiPartStart, normalizedName.Length - episode.NameMultiPartStart );
            normalizedName.AppendFormat( "Part {0}", episode.MultiPartNumber );
         }

         var invalidChararacters = Path.GetInvalidFileNameChars();

         foreach ( var invalidCharacter in invalidChararacters )
         {
            normalizedName.Replace( invalidCharacter, '_' );
         }

         return normalizedName.ToString();
      }

      private void RemovePartSuffixFromDissimilarEpisodeNames( IList<Episode> episodeList )
      {
         for ( var i = 0; i < episodeList.Count; i++ )
         {
            if ( episodeList[i].IsMultiPart )
            {
               if ( i + 1 < episodeList.Count )
               {
                  if ( episodeList[i + 1].IsMultiPart )
                  {
                     if ( String.Compare( episodeList[i].NameBase, episodeList[i + 1].NameBase, true ) != 0 )
                     {
                        episodeList[i].FileName = Episode.StripMultiPartSuffix( episodeList[i].FileName );
                        episodeList[i + 1].FileName = Episode.StripMultiPartSuffix( episodeList[i + 1].FileName );
                        i++;
                     }
                  }
               }
            }
         }
      }

      private string MakeMultiPartFileName( int firstPartIndex, IList<Episode> episodeList )
      {
         var multiPartId = episodeList[firstPartIndex].MultiPartId;
         var lastPartIndex = firstPartIndex;
         var regex = new Regex( @".+? \(\d+\)$" );

         for ( var i = firstPartIndex + 1; i < episodeList.Count; i++ )
         {
            if ( episodeList[i].IsMultiPart )
            {
               lastPartIndex = i;
            }
            else
            {
               break;
            }
         }

         var fileName = String.Format( "S{0:D2}E{1:D2}-E{2:D2} {3}", episodeList[firstPartIndex].SeasonNumber, episodeList[firstPartIndex].EpisodeNumber, episodeList[lastPartIndex].EpisodeNumber, episodeList[firstPartIndex].NameBase );
         return fileName;
      }
   }
}
