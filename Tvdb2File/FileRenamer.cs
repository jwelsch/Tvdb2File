//////////////////////////////////////////////////////////////////////////////
// <copyright file="FileRenamer.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

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

      public int RenameSeasonEpisodeFiles( string seasonDirectoryPath, IList<Episode> episodeList, bool dryRun )
      {
         var episodeFileNames = Directory.GetFiles( seasonDirectoryPath, "*.mp4", SearchOption.TopDirectoryOnly );

         if ( episodeFileNames.Length != episodeList.Count )
         {
            throw new UnexpectedEpisodeCountException( String.Format( "The number of episode files ({0}) does not match the number of episodes from thetvdb.com ({1}).", episodeFileNames.Length, episodeList.Count ) );
         }

         var newFileNames = this.CreateNewFileNameList( episodeList );
         this.CorrectNewFileNameList( newFileNames );

         Array.Sort<string>( episodeFileNames, new NaturalStringComparer() );

         for ( var i = 0; i < episodeFileNames.Length; i++ )
         {
            var targetFileNameWithExt = newFileNames[i] + Path.GetExtension( episodeFileNames[i] );
            var targetFilePath = Path.Combine( Path.GetDirectoryName( episodeFileNames[i] ), targetFileNameWithExt );

            if ( !dryRun )
            {
               // TODO: Uncomment when done testing!!
               File.Move( episodeFileNames[i], targetFilePath );
            }

            if ( this.FileRenamed != null )
            {
               this.FileRenamed( this, new FileRenamedArgs( Path.GetFileName( episodeFileNames[i] ), targetFileNameWithExt ) );
            }
         }

         return episodeFileNames.Length;
      }

      private string[] CreateNewFileNameList( IList<Episode> episodeList )
      {
         var newFileNameList = new List<string>();

         for ( var i = 0; i < episodeList.Count; i++ )
         {
            var normalizedName = this.NormalizeEpisodeName( episodeList[i].Name );
            var targetFileName = String.Format( "S{0:D2}E{1:D2} {2}", episodeList[i].SeasonNumber, episodeList[i].EpisodeNumber, normalizedName );
            newFileNameList.Add( targetFileName );
         }

         return newFileNameList.ToArray();
      }

      private string NormalizeEpisodeName( string episodeName )
      {
         var normalizedName = new StringBuilder( episodeName );
         var findRegex = new Regex( @".+? \(\d+\)$" );

         if ( findRegex.IsMatch( episodeName ) )
         {
            var renameRegex = new Regex( @"\(\d+\)" );
            var match = renameRegex.Match( episodeName );
            var number = match.Value.Substring( 1, match.Value.Length - 2 );
            normalizedName = new StringBuilder( renameRegex.Replace( episodeName, String.Format( "Part {0}", number ) ) );
         }

         var invalidChararacters = Path.GetInvalidFileNameChars();

         foreach ( var invalidCharacter in invalidChararacters )
         {
            normalizedName.Replace( invalidCharacter, '_' );
         }

         return normalizedName.ToString();
      }

      private void CorrectNewFileNameList( string[] newFileNameList )
      {
         for ( var i = 0; i < newFileNameList.Length; i++ )
         {
            var regex = new Regex( @" Part \d+$" );
            var firstMatch = regex.Match( newFileNameList[i] );

            if ( firstMatch.Success )
            {
               if ( i + 1 < newFileNameList.Length )
               {
                  var secondMatch = regex.Match( newFileNameList[i + 1] );

                  if ( secondMatch.Success )
                  {
                     var offset = 7;
                     var firstBase = newFileNameList[i].Substring( offset, firstMatch.Index - firstMatch.Length );
                     var secondBase = newFileNameList[i + 1].Substring( offset, secondMatch.Index - secondMatch.Length );

                     if ( String.Compare( firstBase, secondBase, true ) != 0 )
                     {
                        newFileNameList[i] = regex.Replace( newFileNameList[i], string.Empty );
                        newFileNameList[i + 1] = regex.Replace( newFileNameList[i + 1], string.Empty );
                        i++;
                     }
                  }
               }
            }
         }
      }
   }
}
