//////////////////////////////////////////////////////////////////////////////
// <copyright file="Exceptions.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////
      

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tvdb2File
{
   #region CommandLineException

   /// <summary>
   /// Thrown when there is an error with the command line arguments.
   /// </summary>
   [Serializable]
   public class CommandLineException : Exception
   {
      /// <summary>
      /// Constructs an object of type CommandLineException.
      /// </summary>
      public CommandLineException()
         : this( string.Empty, null )
      {
      }

      /// <summary>
      /// Constructs an object of type CommandLineException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      public CommandLineException( string message )
         : this( message, null )
      {
      }

      /// <summary>
      /// Constructs an object of type CommandLineException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      /// <param name="innerException">Another exception associated with the CommandLineException.</param>
      public CommandLineException( string message, Exception innerException )
         : base( message, innerException )
      {
      }

      /// <summary>
      /// Constructs a CommandLineException object.
      /// </summary>
      /// <param name="info">The serialization information.</param>
      /// <param name="context">The streaming context.</param>
      protected CommandLineException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context )
         : base( info, context )
      {
      }
   }

   #endregion

   #region UnexpectedEpisodeCountException

   /// <summary>
   /// Thrown when there is an error with the command line arguments.
   /// </summary>
   [Serializable]
   public class UnexpectedEpisodeCountException : Exception
   {
      /// <summary>
      /// Constructs an object of type UnexpectedEpisodeCountException.
      /// </summary>
      public UnexpectedEpisodeCountException()
         : this( string.Empty, null )
      {
      }

      /// <summary>
      /// Constructs an object of type UnexpectedEpisodeCountException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      public UnexpectedEpisodeCountException( string message )
         : this( message, null )
      {
      }

      /// <summary>
      /// Constructs an object of type UnexpectedEpisodeCountException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      /// <param name="innerException">Another exception associated with the UnexpectedEpisodeCountException.</param>
      public UnexpectedEpisodeCountException( string message, Exception innerException )
         : base( message, innerException )
      {
      }

      /// <summary>
      /// Constructs a UnexpectedEpisodeCountException object.
      /// </summary>
      /// <param name="info">The serialization information.</param>
      /// <param name="context">The streaming context.</param>
      protected UnexpectedEpisodeCountException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context )
         : base( info, context )
      {
      }
   }

   #endregion

   #region MultipleSeriesReturnedException

   /// <summary>
   /// Thrown when there is an error with the command line arguments.
   /// </summary>
   [Serializable]
   public class MultipleSeriesReturnedException : Exception
   {
      public ReadOnlyCollection<string> SeriesReturned;

      /// <summary>
      /// Constructs an object of type MultipleSeriesReturnedException.
      /// </summary>
      /// <param name="seriesReturned">A list of the names of series that were returned.</param>
      public MultipleSeriesReturnedException( IList<string> seriesReturned )
         : this( String.Empty, seriesReturned, null )
      {
      }

      /// <summary>
      /// Constructs an object of type MultipleSeriesReturnedException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      /// <param name="seriesReturned">A list of the names of series that were returned.</param>
      public MultipleSeriesReturnedException( string message, IList<string> seriesReturned )
         : this( message, seriesReturned, null )
      {
      }

      /// <summary>
      /// Constructs an object of type MultipleSeriesReturnedException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      /// <param name="seriesReturned">A list of the names of series that were returned.</param>
      /// <param name="innerException">Another exception associated with the MultipleSeriesReturnedException.</param>
      public MultipleSeriesReturnedException( string message, IList<string> seriesReturned, Exception innerException )
         : base( message, innerException )
      {
         this.SeriesReturned = new ReadOnlyCollection<string>( seriesReturned );
      }

      /// <summary>
      /// Constructs a MultipleSeriesReturnedException object.
      /// </summary>
      /// <param name="info">The serialization information.</param>
      /// <param name="context">The streaming context.</param>
      protected MultipleSeriesReturnedException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context )
         : base( info, context )
      {
      }
   }

   #endregion

   #region XmlFormatException

   /// <summary>
   /// Thrown when there is an error with the command line arguments.
   /// </summary>
   [Serializable]
   public class XmlFormatException : Exception
   {
      /// <summary>
      /// Constructs an object of type XmlFormatException.
      /// </summary>
      public XmlFormatException()
         : this( string.Empty, null )
      {
      }

      /// <summary>
      /// Constructs an object of type XmlFormatException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      public XmlFormatException( string message )
         : this( message, null )
      {
      }

      /// <summary>
      /// Constructs an object of type XmlFormatException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      /// <param name="innerException">Another exception associated with the XmlFormatException.</param>
      public XmlFormatException( string message, Exception innerException )
         : base( message, innerException )
      {
      }

      /// <summary>
      /// Constructs a XmlFormatException object.
      /// </summary>
      /// <param name="info">The serialization information.</param>
      /// <param name="context">The streaming context.</param>
      protected XmlFormatException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context )
         : base( info, context )
      {
      }
   }

   #endregion
}