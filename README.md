# Introduction
Tvdb2File is a .NET command line program that renames MP4 files after episodes.

# Copyright
Copyright (c) Justin Welsch
All rights reserved.

# Objective
Retrieve TV series episode information from thetvdb.com and use it to automatically name files in the local file system.

# Project Parameters
- Only one season can be processed at a time.
- The app expects a certain directory structure.
- The app will only rename MP4 files.
- MP4 files are expected to be in the same order as they appear in the season.

# File System Structure
This is an example of the structure of the file system containing series, seasons, and episodes.

```
.\
 |- Series A
           |- Season 1
                     |- Episode 1.mp4
                     |- ...
                     |- Episode n.mp4
           |- ...
           |- Season n
 |- Series B
           |- Season 1
                     |- Episode 1.mp4
                     |- ...
                     |- Episode n.mp4
           |- ...
           |- Season n
```

# Command Line Arguments

```
Tvdb2File.exe -season "path to season" [[-search "series search term"]|[-seriesId xxxxx]] [-forceUpdate] [-collapseMultiPart]
  -season: (Mandatory) Relative or absolute path to the directory containing the season to rename.
  -search: (Optional) Terms to use to search for the series.  If not supplied, the name of the series directory will be used.  This argument is mutually exclusive with -seriesId.
  -seriesId: (Optional) ID of the series to use for episode naming.  This argument is mutually exclusive with -search.
  -forceUpdate: (Optional) Include to force an update of the local episode database from thetvdb.com.
  -dryRun: (Optional) Does everything except do the actual file renaming.
  -collapseMultiPart: (Optional) Attempts to automatically collapse episodes specified as multipart into one local file.
```
