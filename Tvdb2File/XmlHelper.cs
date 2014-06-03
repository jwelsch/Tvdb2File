//////////////////////////////////////////////////////////////////////////////
// <copyright file="XmlHelper.cs" owner="Justin Welsch">
// Copyright (c) 2014 All Rights Reserved
// <author>Justin Welsch</author>
// </copyright>
//////////////////////////////////////////////////////////////////////////////
      

using System;
using System.Xml;

namespace Tvdb2File
{
   /// <summary>
   /// Helps with XML.
   /// </summary>
   internal static class XmlHelper
   {
      /// <summary>
      /// Checks that the next element is present and what is expected.
      /// </summary>
      /// <param name="xmlReader">Reads XML.</param>
      /// <param name="element">Element to check for.</param>
      /// <param name="nodeType">The node type to check.</param>
      /// <param name="throwException">True to throw an exception if the element does not exist, false otherwise.</param>
      /// <returns>True if the element exists, false otherwise.</returns>
      public static bool CheckNextElement( XmlReader xmlReader, string element, XmlNodeType nodeType, bool throwException = true )
      {
         // Make sure there is a next element.
         if ( !xmlReader.Read() )
         {
            if ( throwException )
            {
               XmlHelper.ThrowMissingElement( element, nodeType );
            }

            return false;
         }

         // Check that the next element is what is expected.
         if ( ( xmlReader.Name != element ) || ( xmlReader.NodeType != nodeType ) )
         {
            if ( throwException )
            {
               XmlHelper.ThrowExpectedElement( element, nodeType );
            }

            return false;
         }

         return true;
      }

      /// <summary>
      /// Checks that the current element is what is expected.
      /// </summary>
      /// <param name="xmlReader">Reads XML.</param>
      /// <param name="element">Element to check for.</param>
      /// <param name="nodeType">The node type to check.</param>
      /// <param name="throwException">True to throw an exception if the element does not exist, false otherwise.</param>
      /// <returns>True if the element exists, false otherwise.</returns>
      public static bool CheckElement( XmlReader xmlReader, string element, XmlNodeType nodeType, bool throwException = true )
      {
         // Check that the next element is what is expected.
         if ( ( xmlReader.Name != element ) || ( xmlReader.NodeType != nodeType ) )
         {
            if ( throwException )
            {
               XmlHelper.ThrowExpectedElement( element, nodeType );
            }

            return false;
         }

         return true;
      }

      /// <summary>
      /// Throws an XmlException indicating than an element is missing.
      /// </summary>
      /// <param name="missingElement">Name of the missing element.</param>
      /// <param name="nodeType">Type of node that was missing.</param>
      public static void ThrowMissingElement( string missingElement, XmlNodeType nodeType )
      {
         throw new XmlException( String.Format( "Missing \"{0}\" element of type \"{1}\".", missingElement, nodeType ) );
      }

      /// <summary>
      /// Throws an XmlException indicating than an expected element was not found.
      /// </summary>
      /// <param name="expectedElement">Name of the expected element.</param>
      /// <param name="nodeType">Type of node that was missing.</param>
      public static void ThrowExpectedElement( string expectedElement, XmlNodeType nodeType )
      {
         throw new XmlException( String.Format( "Expected \"{0}\" element of type \"{1}\".", expectedElement, nodeType ) );
      }

      /// <summary>
      /// Throws an XmlException indicating that the element did not contain any text.
      /// </summary>
      /// <param name="containerElement">Element containing the missing content.</param>
      public static void ThrowEmptyString( string containerElement )
      {
         throw new XmlException( String.Format( "The element \"{0}\" did not contain any text.", containerElement ) );
      }

      /// <summary>
      /// Converts text into a value of the specified type.
      /// </summary>
      /// <typeparam name="T">Type that the text will be converted to.</typeparam>
      /// <param name="text">Text to convert.</param>
      /// <returns>The value as the specified type.</returns>
      private delegate T ConvertText<T>( string text );

      /// <summary>
      /// Reads text as the specified type.
      /// </summary>
      /// <typeparam name="T">Type that the text will be converted to.</typeparam>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <param name="converter">Converts the text to the specified type.</param>
      /// <returns>The value as the specified type.</returns>
      private static T ReadStringAs<T>( XmlReader xmlReader, ConvertText<T> converter )
      {
         var element = xmlReader.Name;

         var text = xmlReader.ReadString();

         if ( text == String.Empty )
         {
            XmlHelper.ThrowEmptyString( element );
         }

         try
         {
            return converter( text );
         }
         catch ( Exception ex )
         {
            throw new XmlException( String.Format( "Failed to convert width to {0}.", typeof( T ) ), ex );
         }
      }

      /// <summary>
      /// Reads the text as an Boolean.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as an Boolean.</returns>
      public static Boolean ReadStringAsBoolean( XmlReader xmlReader )
      {
         return XmlHelper.ReadStringAs<Boolean>( xmlReader, Boolean.Parse );
      }

      /// <summary>
      /// Reads the text as an Char.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as an Char.</returns>
      public static Char ReadStringAsChar( XmlReader xmlReader )
      {
         return XmlHelper.ReadStringAs<Char>( xmlReader, Char.Parse );
      }

      /// <summary>
      /// Reads the text as an Byte.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as an Byte.</returns>
      public static Byte ReadStringAsByte( XmlReader xmlReader )
      {
         return XmlHelper.ReadStringAs<Byte>( xmlReader, Byte.Parse );
      }

      /// <summary>
      /// Reads the text as an SByte.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as an SByte.</returns>
      public static SByte ReadStringAsSByte( XmlReader xmlReader )
      {
         return XmlHelper.ReadStringAs<SByte>( xmlReader, SByte.Parse );
      }

      /// <summary>
      /// Reads the text as an Int16.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as an Int16.</returns>
      public static Int16 ReadStringAsInt16( XmlReader xmlReader )
      {
         return XmlHelper.ReadStringAs<Int16>( xmlReader, Int16.Parse );
      }

      /// <summary>
      /// Reads the text as an UInt16.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as an UInt16.</returns>
      public static UInt16 ReadStringAsUInt16( XmlReader xmlReader )
      {
         return XmlHelper.ReadStringAs<UInt16>( xmlReader, UInt16.Parse );
      }

      /// <summary>
      /// Reads the text as an Int32.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as an Int32.</returns>
      public static Int32 ReadStringAsInt32( XmlReader xmlReader )
      {
         return XmlHelper.ReadStringAs<Int32>( xmlReader, Int32.Parse );
      }

      /// <summary>
      /// Reads the text as an UInt32.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as an UInt32.</returns>
      public static UInt32 ReadStringAsUInt32( XmlReader xmlReader )
      {
         return XmlHelper.ReadStringAs<UInt32>( xmlReader, UInt32.Parse );
      }

      /// <summary>
      /// Reads the text as a Single.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as a Single.</returns>
      public static Single ReadStringAsSingle( XmlReader xmlReader )
      {
         return XmlHelper.ReadStringAs<Single>( xmlReader, Single.Parse );
      }

      /// <summary>
      /// Reads the text as a Double.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as a Double.</returns>
      public static Double ReadStringAsDouble( XmlReader xmlReader )
      {
         return XmlHelper.ReadStringAs<Double>( xmlReader, Double.Parse );
      }

      /// <summary>
      /// Reads the text as a Decimal.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as a Decimal.</returns>
      public static Decimal ReadStringAsDecimal( XmlReader xmlReader )
      {
         return XmlHelper.ReadStringAs<Decimal>( xmlReader, Decimal.Parse );
      }

      /// <summary>
      /// Reads the text.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as a string.</returns>
      public static String ReadString( XmlReader xmlReader )
      {
         return xmlReader.ReadString();
      }

      /// <summary>
      /// Reads the text as a DateTime.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as a DateTime.</returns>
      public static DateTime ReadStringAsDateTime( XmlReader xmlReader )
      {
         return XmlHelper.ReadStringAs<DateTime>( xmlReader, DateTime.Parse );
      }

      /// <summary>
      /// Reads the text as a TimeSpan.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <returns>Value as a TimeSpan.</returns>
      public static TimeSpan ReadStringAsTimeSpan( XmlReader xmlReader )
      {
         return XmlHelper.ReadStringAs<TimeSpan>( xmlReader, TimeSpan.Parse );
      }

      /// <summary>
      /// Checks that the attribute exists.
      /// </summary>
      /// <param name="xmlReader">Reads XML from persistent storage.</param>
      /// <param name="attributeName">Name of the attribute.</param>
      /// <param name="throwException">True to throw an exception if the element does not exist, false otherwise.</param>
      /// <returns>True if the element exists, false otherwise.</returns>
      public static bool CheckAttribute( XmlReader xmlReader, string attributeName, bool throwException = true )
      {
         var containerElement = xmlReader.Name;

         if ( !xmlReader.MoveToAttribute( attributeName ) )
         {
            if ( throwException )
            {
               XmlHelper.ThrowMissingAttribute( containerElement, attributeName );
            }

            return false;
         }

         return true;
      }

      /// <summary>
      /// Throws an XmlException indicating than an attribute is missing in the specified element.
      /// </summary>
      /// <param name="containerElement">Name of the element that should contain the missing attribute.</param>
      /// <param name="attributeName">Name of the attribute that is missing.</param>
      public static void ThrowMissingAttribute( string containerElement, string attributeName )
      {
         throw new XmlException( String.Format( "Missing attribute \"{0}\" in \"{1}\" element.", attributeName, containerElement ) );
      }

      /// <summary>
      /// Writes the specified value surrounded by the specified XML element.
      /// </summary>
      /// <param name="xmlWriter">XmlWriter to use.</param>
      /// <param name="xmlElementName">Name of the surrounding element.</param>
      /// <param name="value">Value to surround.</param>
      public static void WriteEnclosedValue( XmlWriter xmlWriter, string xmlElementName, System.String value )
      {
         xmlWriter.WriteStartElement( xmlElementName );
         xmlWriter.WriteValue( value );
         xmlWriter.WriteEndElement();
      }

      /// <summary>
      /// Writes the specified value surrounded by the specified XML element.
      /// </summary>
      /// <param name="xmlWriter">XmlWriter to use.</param>
      /// <param name="xmlElementName">Name of the surrounding element.</param>
      /// <param name="value">Value to surround.</param>
      public static void WriteEnclosedValue( XmlWriter xmlWriter, string xmlElementName, System.Boolean value )
      {
         XmlHelper.WriteEnclosedValue( xmlWriter, xmlElementName, value.ToString() );
      }

      /// <summary>
      /// Writes the specified value surrounded by the specified XML element as RFC1123 format.
      /// </summary>
      /// <param name="xmlWriter">XmlWriter to use.</param>
      /// <param name="xmlElementName">Name of the surrounding element.</param>
      /// <param name="value">Value to surround.</param>
      public static void WriteEnclosedValue( XmlWriter xmlWriter, string xmlElementName, System.DateTime value )
      {
         XmlHelper.WriteEnclosedValue( xmlWriter, xmlElementName, value.ToString( "R" ) );
      }

      /// <summary>
      /// Writes the specified value surrounded by the specified XML element.
      /// </summary>
      /// <param name="xmlWriter">XmlWriter to use.</param>
      /// <param name="xmlElementName">Name of the surrounding element.</param>
      /// <param name="value">Value to surround.</param>
      /// <param name="stringFormat">Format to write out the string.</param>
      public static void WriteEnclosedValue( XmlWriter xmlWriter, string xmlElementName, System.DateTime value, string stringFormat )
      {
         XmlHelper.WriteEnclosedValue( xmlWriter, xmlElementName, value.ToString( stringFormat ) );
      }

      /// <summary>
      /// Writes the specified value surrounded by the specified XML element.
      /// </summary>
      /// <param name="xmlWriter">XmlWriter to use.</param>
      /// <param name="xmlElementName">Name of the surrounding element.</param>
      /// <param name="value">Value to surround.</param>
      public static void WriteEnclosedValue( XmlWriter xmlWriter, string xmlElementName, System.TimeSpan value )
      {
         XmlHelper.WriteEnclosedValue( xmlWriter, xmlElementName, value.ToString() );
      }

      /// <summary>
      /// Writes the specified value surrounded by the specified XML element.
      /// </summary>
      /// <param name="xmlWriter">XmlWriter to use.</param>
      /// <param name="xmlElementName">Name of the surrounding element.</param>
      /// <param name="value">Value to surround.</param>
      public static void WriteEnclosedValue( XmlWriter xmlWriter, string xmlElementName, System.Decimal value )
      {
         XmlHelper.WriteEnclosedValue( xmlWriter, xmlElementName, value.ToString() );
      }

      /// <summary>
      /// Writes the specified value surrounded by the specified XML element.
      /// </summary>
      /// <param name="xmlWriter">XmlWriter to use.</param>
      /// <param name="xmlElementName">Name of the surrounding element.</param>
      /// <param name="value">Value to surround.</param>
      public static void WriteEnclosedValue( XmlWriter xmlWriter, string xmlElementName, System.Double value )
      {
         XmlHelper.WriteEnclosedValue( xmlWriter, xmlElementName, value.ToString() );
      }

      /// <summary>
      /// Writes the specified value surrounded by the specified XML element.
      /// </summary>
      /// <param name="xmlWriter">XmlWriter to use.</param>
      /// <param name="xmlElementName">Name of the surrounding element.</param>
      /// <param name="value">Value to surround.</param>
      public static void WriteEnclosedValue( XmlWriter xmlWriter, string xmlElementName, System.Single value )
      {
         XmlHelper.WriteEnclosedValue( xmlWriter, xmlElementName, value.ToString() );
      }

      /// <summary>
      /// Writes the specified value surrounded by the specified XML element.
      /// </summary>
      /// <param name="xmlWriter">XmlWriter to use.</param>
      /// <param name="xmlElementName">Name of the surrounding element.</param>
      /// <param name="value">Value to surround.</param>
      public static void WriteEnclosedValue( XmlWriter xmlWriter, string xmlElementName, System.Int32 value )
      {
         XmlHelper.WriteEnclosedValue( xmlWriter, xmlElementName, value.ToString() );
      }

      /// <summary>
      /// Writes the specified value surrounded by the specified XML element.
      /// </summary>
      /// <param name="xmlWriter">XmlWriter to use.</param>
      /// <param name="xmlElementName">Name of the surrounding element.</param>
      /// <param name="value">Value to surround.</param>
      public static void WriteEnclosedValue( XmlWriter xmlWriter, string xmlElementName, System.Int64 value )
      {
         XmlHelper.WriteEnclosedValue( xmlWriter, xmlElementName, value.ToString() );
      }

      /// <summary>
      /// Writes the specified value surrounded by the specified XML element.
      /// </summary>
      /// <param name="xmlWriter">XmlWriter to use.</param>
      /// <param name="xmlElementName">Name of the surrounding element.</param>
      /// <param name="value">Value to surround.</param>
      public static void WriteEnclosedValue( XmlWriter xmlWriter, string xmlElementName, System.Object value )
      {
         XmlHelper.WriteEnclosedValue( xmlWriter, xmlElementName, value.ToString() );
      }
   }
}
