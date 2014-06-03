//////////////////////////////////////////////////////////////////////////////
// <copyright file="Extensions.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////
      

using System;

namespace Tvdb2File
{
   public static class Extensions
   {
      public static bool BitFlagSet( this UInt32 left, UInt32 flag )
      {
         return ( left & flag ) == flag;
      }

      public static bool BitFlagSet( this Int32 left, Int32 flag )
      {
         return ( left & flag ) == flag;
      }
   }
}
