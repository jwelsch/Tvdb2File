﻿//////////////////////////////////////////////////////////////////////////////
// <copyright file="NaturalSortComparer.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////
      

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace Tvdb2File
{
   [SuppressUnmanagedCodeSecurity]
   internal static class SafeNativeMethods
   {
      [DllImport( "shlwapi.dll", CharSet = CharSet.Unicode )]
      public static extern int StrCmpLogicalW( string psz1, string psz2 );
   }

   public sealed class NaturalStringComparer : IComparer<string>
   {
      public int Compare( string a, string b )
      {
         return SafeNativeMethods.StrCmpLogicalW( a, b );
      }
   }

   public sealed class NaturalFileInfoNameComparer : IComparer<FileInfo>
   {
      public int Compare( FileInfo a, FileInfo b )
      {
         return SafeNativeMethods.StrCmpLogicalW( a.Name, b.Name );
      }
   }

   //      public class NaturalSortComparer<T> : IComparer<string>, IDisposable
   //      {
   //private bool isAscending;

   //public NaturalSortComparer( bool inAscendingOrder = true )
   //{
   //   this.isAscending = inAscendingOrder;
   //}

   //#region IComparer<string> Members

   //public int Compare( string x, string y )
   //{
   //   throw new NotImplementedException();
   //}

   //#endregion

   //#region IComparer<string> Members

   //int IComparer<string>.Compare( string x, string y )
   //{
   //   if ( x == y )
   //      return 0;

   //   string[] x1, y1;

   //   if ( !table.TryGetValue( x, out x1 ) )
   //   {
   //      x1 = Regex.Split( x.Replace( " ", "" ), "([0-9]+)" );
   //      table.Add( x, x1 );
   //   }

   //   if ( !table.TryGetValue( y, out y1 ) )
   //   {
   //      y1 = Regex.Split( y.Replace( " ", "" ), "([0-9]+)" );
   //      table.Add( y, y1 );
   //   }

   //   int returnVal;

   //   for ( int i = 0; i < x1.Length && i < y1.Length; i++ )
   //   {
   //      if ( x1[i] != y1[i] )
   //      {
   //         returnVal = PartCompare( x1[i], y1[i] );
   //         return isAscending ? returnVal : -returnVal;
   //      }
   //   }

   //   if ( y1.Length > x1.Length )
   //   {
   //      returnVal = 1;
   //   }
   //   else if ( x1.Length > y1.Length )
   //   {
   //      returnVal = -1;
   //   }
   //   else
   //   {
   //      returnVal = 0;
   //   }

   //   return isAscending ? returnVal : -returnVal;
   //}

   //private static int PartCompare( string left, string right )
   //{
   //   int x, y;
   //   if ( !int.TryParse( left, out x ) )
   //      return left.CompareTo( right );

   //   if ( !int.TryParse( right, out y ) )
   //      return left.CompareTo( right );

   //   return x.CompareTo( y );
   //}

   //#endregion

   //private Dictionary<string, string[]> table = new Dictionary<string, string[]>();

   //public void Dispose()
   //{
   //   table.Clear();
   //   table = null;
   //}
   //   }
}
