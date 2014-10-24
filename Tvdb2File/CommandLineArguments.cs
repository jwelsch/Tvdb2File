using System;
using CommandLineLib;

namespace Tvdb2File
{
   public class CommandLineArguments
   {
      public static int NoSeriesId = -1;

      public CommandLineArguments()
      {
         this.SeriesId = CommandLineArguments.NoSeriesId;
      }

      [DirectoryPathCompound( "-season", MustExist = true, ShortName = "Season Path", Description = "Relative or absolute path to the directory containing the season to rename." )]
      public string SeasonPath
      {
         get;
         private set;
      }

      [StringCompound( "-search", Optional = true, Groups = new int[] { 1 }, ShortName = "Series Search Terms", Description = "Terms to use to search for the series.  If not supplied, the name of the series directory will be used.  This argument is mutually exclusive with -seriesId." )]
      public string SeriesSearchTerms
      {
         get;
         private set;
      }

      [Int32Compound( "-seriesId", Optional = true, Groups = new int[] { 2 }, ShortName = "Series ID", Description = "ID of the series to use for episode naming.  This argument is mutually exclusive with -search." )]
      public int SeriesId
      {
         get;
         private set;
      }

      [Switch( "-forceUpdate", Optional = true, Description = "Include to force an update of the local episode database from thetvdb.com." )]
      public bool ForceUpdate
      {
         get;
         private set;
      }

      [Switch( "-dryRun", Optional = true, Description = "Does everything except do the actual file renaming." )]
      public bool DryRun
      {
         get;
         private set;
      }

      [Switch( "-collapseMultiPart", Optional = true, Description = "Attempts to automatically collapse episodes specified as multipart into one local file." )]
      public bool CollapseMultiPart
      {
         get;
         private set;
      }
   }
}
