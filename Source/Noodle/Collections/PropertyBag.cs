using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Noodle.Collections
{
    /// <summary>
    /// Creates a serializable string/object dictionary that is XML serializable
    /// Encodes keys as element names and values as simple values with a type
    /// attribute that contains an XML type name. Complex names encode the type 
    /// name with type='___namespace.classname' format followed by a standard xml
    /// serialized format. The latter serialization can be slow so it's not recommended
    /// to pass complex types if performance is critical.
    /// </summary>
    [XmlRoot("properties")]
    public class PropertyBag : PropertyBag<object>
    {
        /// <summary>
        /// Creates an instance of a property bag from an Xml string
        /// </summary>
        /// <param name="xml">Serialize</param>
        /// <returns></returns>
        public new static PropertyBag CreateFromXml(string xml)
        {
            var bag = new PropertyBag();
            bag.FromXml(xml);
            return bag;            
        }
    }

    /// <summary>
    /// Creates a serializable string for generic types that is XML serializable.
    /// 
    /// Encodes keys as element names and values as simple values with a type
    /// attribute that contains an XML type name. Complex names encode the type 
    /// name with type='___namespace.classname' format followed by a standard xml
    /// serialized format. The latter serialization can be slow so it's not recommended
    /// to pass complex types if performance is critical.
    /// </summary>
    /// <typeparam name="TValue">Must be a reference type. For value types use type object</typeparam>
    [XmlRoot("properties")]    
    public class PropertyBag<TValue> : Dictionary<string, TValue>, IXmlSerializable               
    {           
        /// <summary>
        /// Not implemented - this means no schema information is passed
        /// so this won't work with ASMX/WCF services.
        /// </summary>
        /// <returns></returns>       
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }


        /// <summary>
        /// Serializes the dictionary to XML. Keys are 
        /// serialized to element names and values as 
        /// element values. An xml type attribute is embedded
        /// for each serialized element - a .NET type
        /// element is embedded for each complex type and
        /// prefixed with three underscores.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (string key in this.Keys)
            {
                TValue value = this[key];

                Type type = null;
                if (value != null)
                    type = value.GetType();

                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                writer.WriteString(key as string);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                string xmlType = CommonHelper.MapTypeToXmlType(type);
                bool isCustom = false;

                // Type information attribute if not string
                if (value == null)
                {
                    writer.WriteAttributeString("type", "nil");
                }
                else if (!string.IsNullOrEmpty(xmlType))
                {
                    if (xmlType != "string")
                    {
                        writer.WriteStartAttribute("type");
                        writer.WriteString(xmlType);
                        writer.WriteEndAttribute();
                    }
                }
                else
                {
                    isCustom = true;
                    xmlType = "___" + value.GetType().FullName;
                    writer.WriteStartAttribute("type");
                    writer.WriteString(xmlType);
                    writer.WriteEndAttribute();
                }

                // Actual deserialization
                if (!isCustom)
                {
                    if (value != null)
                        writer.WriteValue(value);
                }
                else
                {
                    var ser = new XmlSerializer(value.GetType());
                    ser.Serialize(writer, value);
                }
                writer.WriteEndElement(); // value

                writer.WriteEndElement(); // item
            }
        }
        

        /// <summary>
        /// Reads the custom serialized format
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            this.Clear();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "key")
                {                    
                    string xmlType = null;
                    string name = reader.ReadElementContentAsString(); 

                    // item element
                    reader.ReadToNextSibling("value");
                    
                    if (reader.MoveToNextAttribute())
                        xmlType = reader.Value;
                    reader.MoveToContent();

                    TValue value;
                    if (xmlType == "nil")
                        value = default(TValue); // null
                    else if (string.IsNullOrEmpty(xmlType))
                    {
                        // value is a string or object and we can assign TValue to value
                        string strval = reader.ReadElementContentAsString();
                        value = (TValue) Convert.ChangeType(strval, typeof(TValue)); 
                    }
                    else if (xmlType.StartsWith("___"))
                    {
                        while (reader.Read() && reader.NodeType != XmlNodeType.Element)
                        { }

                        Type type = Type.GetType(xmlType.Substring(3)); //ReflectionUtils.GetTypeFromName(xmlType.Substring(3));
                        //value = reader.ReadElementContentAs(type,null);
                        var ser = new XmlSerializer(type);
                        value = (TValue)ser.Deserialize(reader);
                    }
                    else
                        value = (TValue)reader.ReadElementContentAs(CommonHelper.MapXmlTypeToType(xmlType), null);

                    this.Add(name, value);
                }
            }
        }


        /// <summary>
        /// Serializes this dictionary to an XML string
        /// </summary>
        /// <returns>XML String</returns>
        public string ToXml()
        {
            string xml = new Serialization.XmlSerializer().Serialize(this);
            // remove declaration if any
            if(xml.StartsWith("<?xml", StringComparison.InvariantCultureIgnoreCase))
            {
                var firstLine = xml.IndexOf(Environment.NewLine, System.StringComparison.Ordinal) + 2;
                xml = xml.Substring(firstLine, xml.Length - (firstLine));
            }
            return xml;
        }

        /// <summary>
        /// Deserializes from an XML string
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>true or false</returns>
        public void FromXml(string xml)
        {
            this.Clear();

            // if xml string is empty we return an empty dictionary                        
            if (string.IsNullOrEmpty(xml))
                return;

            var result = new Serialization.XmlSerializer().Deserialize<PropertyBag<TValue>>(xml);
            if (result != null)
            {
                foreach (var item in result)
                {
                    this.Add(item.Key, item.Value);
                }
            }
        }


        /// <summary>
        /// Creates an instance of a property bag from an Xml string
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static PropertyBag<TValue> CreateFromXml(string xml)
        {
            var bag = new PropertyBag<TValue>();
            bag.FromXml(xml);
            return bag;
        }
    }
}
