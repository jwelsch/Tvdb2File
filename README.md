Tvdb2File is a .NET command line program that renames MP4 files after episodes.

COPYRIGHT
All rights reserved.

OBJECTIVE
Retrieve TV series episode information from thetvdb.com and use it to automatically name files in the local file system.

PARAMETERS
- Only one season can be processed at a time.
- The app expects a certain directory structure.
- The app will only rename MP4 files.
- MP4 files are expected to be in the same order as they appear in the season.

FILE SYSTEM STRUCTURE
.\
 |- Series A
           |- Season 1
                     |- Episode 1 MP4
                     |- ...
                     |- Episode n MP4
           |- ...
           |- Season n
 |- Series B
           |- Season 1
                     |- Episode 1 MP4
                     |- ...
                     |- Episode n MP4
           |- ...
           |- Season n

COMMAND LINE ARGUMENTS
  Tvdb2File.exe -season "path to season" [[-search "series search term"]|[-seriesId xxxxx]]
    -season: (Mandatory) Relative or absolute path to the directory containing the season to rename.
    -search: (Optional) Terms to use to search for the series.  If not supplied, the name of the series directory will be used.  This argument is mutually exclusive with -seriesId.
    -seriesId: (Optional) ID of the series to use for episode naming.  This argument is mutually exclusive with -search.