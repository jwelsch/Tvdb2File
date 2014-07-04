//////////////////////////////////////////////////////////////////////////////
// <copyright file="Extensions.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////


using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Tvdb2File
{
   public class ComparisonComparer<T> : IComparer<T>, IComparer
   {
      private readonly Comparison<T> comparison;

      public ComparisonComparer( Comparison<T> comparison )
      {
         this.comparison = comparison;
      }

      public int Compare( T item1, T item2 )
      {
         return this.comparison( item1, item2 );
      }

      public int Compare( object item1, object item2 )
      {
         return this.comparison( (T) item1, (T) item2 );
      }
   }

   public static class Extensions
   {
      public delegate int Sorter<T>( T item1, T item2 );

      public static bool BitFlagSet( this UInt32 left, UInt32 flag )
      {
         return ( left & flag ) == flag;
      }

      public static bool BitFlagSet( this Int32 left, Int32 flag )
      {
         return ( left & flag ) == flag;
      }

      public static void Sort<T>( this IList<T> list, Comparison<T> comparison )
      {
         ArrayList.Adapter( (IList) list ).Sort( new ComparisonComparer<T>( comparison ) );
      }

      public static IEnumerable<T> OrderBy<T>( this IEnumerable<T> list, Comparison<T> comparison )
      {
         return list.OrderBy( t => t, new ComparisonComparer<T>( comparison ) );
      }
   }
}
