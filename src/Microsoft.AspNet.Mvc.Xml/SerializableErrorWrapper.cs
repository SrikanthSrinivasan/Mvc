// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc.Xml
{
    /// <summary>
    /// Wrapper class for <see cref="SerializableError"/> to enable it to be serialized by the xml formatters.
    /// </summary>
    [XmlRoot("Error")]
    public sealed class SerializableErrorWrapper : IXmlSerializable, IUnwrappable
    {
        // Note: XmlSerializer requires to have default constructor
        public SerializableErrorWrapper()
        {
            SerializableError = new SerializableError();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableErrorWrapper"/> class.
        /// </summary>
        /// <param name="error">The <see cref="SerializableError"/> object that needs to be wrapped.</param>
        public SerializableErrorWrapper([NotNull] SerializableError error)
        {
            SerializableError = error;
        }

        /// <summary>
        /// Gets the wrapped object which is serialized/deserialized into XML
        /// representation.
        /// </summary>
        public SerializableError SerializableError { get; }

        /// <inheritdoc />
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates a <see cref="SerializableError"/> object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }

            reader.ReadStartElement();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                var key = XmlConvert.DecodeName(reader.LocalName);
                var value = reader.ReadInnerXml();

                SerializableError.Add(key, value);
                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        /// <summary>
        /// Converts the wrapped <see cref="SerializableError"/> object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            foreach (var keyValuePair in SerializableError)
            {
                var key = keyValuePair.Key;
                var value = keyValuePair.Value;
                writer.WriteStartElement(XmlConvert.EncodeLocalName(key));
                if (value != null)
                {
                    writer.WriteValue(value);
                }

                writer.WriteEndElement();
            }
        }

        /// <inheritdoc />
        public object Unwrap([NotNull] Type declaredType)
        {
            return SerializableError;
        }
    }
}