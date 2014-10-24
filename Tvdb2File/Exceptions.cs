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
   #region Tvdb2FileException

   /// <summary>
   /// Base class for exceptions defined by Tvdb2File.
   /// </summary>
   [Serializable]
   public class Tvdb2FileException : Exception
   {
      /// <summary>
      /// Constructs an object of type Tvdb2FileException.
      /// </summary>
      public Tvdb2FileException()
      {
      }

      /// <summary>
      /// Constructs an object of type Tvdb2FileException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      public Tvdb2FileException( string message )
         : this( message, null )
      {
      }

      /// <summary>
      /// Constructs an object of type Tvdb2FileException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      /// <param name="innerException">Another exception associated with the Tvdb2FileException.</param>
      public Tvdb2FileException( string message, Exception innerException )
         : base( message, innerException )
      {
      }

      /// <summary>
      /// Constructs a Tvdb2FileException object.
      /// </summary>
      /// <param name="info">The serialization information.</param>
      /// <param name="context">The streaming context.</param>
      protected Tvdb2FileException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context )
         : base( info, context )
      {
      }
   }

   #endregion

   #region UnexpectedEpisodeCountException

   /// <summary>
   /// Thrown when the number of episodes returned from thetvdb.com does match the number of MP4 files in the season directory.
   /// </summary>
   [Serializable]
   public class UnexpectedEpisodeCountException : Tvdb2FileException
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
   /// Thrown when there is more than one series returned.
   /// </summary>
   [Serializable]
   public class MultipleSeriesReturnedException : Tvdb2FileException
   {
      public ReadOnlyCollection<Series> SeriesReturned;

      /// <summary>
      /// Constructs an object of type MultipleSeriesReturnedException.
      /// </summary>
      /// <param name="seriesReturned">A list of the names of series that were returned.</param>
      public MultipleSeriesReturnedException( IList<Series> seriesReturned )
         : this( String.Empty, seriesReturned, null )
      {
      }

      /// <summary>
      /// Constructs an object of type MultipleSeriesReturnedException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      /// <param name="seriesReturned">A list of the names of series that were returned.</param>
      public MultipleSeriesReturnedException( string message, IList<Series> seriesReturned )
         : this( message, seriesReturned, null )
      {
      }

      /// <summary>
      /// Constructs an object of type MultipleSeriesReturnedException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      /// <param name="seriesReturned">A list of the names of series that were returned.</param>
      /// <param name="innerException">Another exception associated with the MultipleSeriesReturnedException.</param>
      public MultipleSeriesReturnedException( string message, IList<Series> seriesReturned, Exception innerException )
         : base( message, innerException )
      {
         this.SeriesReturned = new ReadOnlyCollection<Series>( seriesReturned );
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

   #region NoSeriesFoundException

   /// <summary>
   /// Thrown when no series is found.
   /// </summary>
   [Serializable]
   public class NoSeriesFoundException : Tvdb2FileException
   {
      /// <summary>
      /// Constructs an object of type NoSeriesFoundException.
      /// </summary>
      public NoSeriesFoundException()
         : this( string.Empty, null )
      {
      }

      /// <summary>
      /// Constructs an object of type NoSeriesFoundException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      public NoSeriesFoundException( string message )
         : this( message, null )
      {
      }

      /// <summary>
      /// Constructs an object of type NoSeriesFoundException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      /// <param name="innerException">Another exception associated with the NoSeriesFoundException.</param>
      public NoSeriesFoundException( string message, Exception innerException )
         : base( message, innerException )
      {
      }

      /// <summary>
      /// Constructs a NoSeriesFoundException object.
      /// </summary>
      /// <param name="info">The serialization information.</param>
      /// <param name="context">The streaming context.</param>
      protected NoSeriesFoundException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context )
         : base( info, context )
      {
      }
   }

   #endregion

   #region NoEpisodesFoundException

   /// <summary>
   /// Thrown when no episodes are found.
   /// </summary>
   [Serializable]
   public class NoEpisodesFoundException : Tvdb2FileException
   {
      /// <summary>
      /// Constructs an object of type NoEpisodesFoundException.
      /// </summary>
      public NoEpisodesFoundException()
         : this( string.Empty, null )
      {
      }

      /// <summary>
      /// Constructs an object of type NoEpisodesFoundException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      public NoEpisodesFoundException( string message )
         : this( message, null )
      {
      }

      /// <summary>
      /// Constructs an object of type NoEpisodesFoundException.
      /// </summary>
      /// <param name="message">The message associated with the exception.</param>
      /// <param name="innerException">Another exception associated with the NoEpisodesFoundException.</param>
      public NoEpisodesFoundException( string message, Exception innerException )
         : base( message, innerException )
      {
      }

      /// <summary>
      /// Constructs a NoEpisodesFoundException object.
      /// </summary>
      /// <param name="info">The serialization information.</param>
      /// <param name="context">The streaming context.</param>
      protected NoEpisodesFoundException( System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context )
         : base( info, context )
      {
      }
   }

   #endregion

   #region XmlFormatException

   /// <summary>
   /// Thrown when there is an error with the XML format.
   /// </summary>
   [Serializable]
   public class XmlFormatException : Tvdb2FileException
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