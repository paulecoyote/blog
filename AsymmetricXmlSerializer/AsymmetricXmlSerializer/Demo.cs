// See Clue class to change demo behaviour.  
// Define / search for FIX1 and FIX2
//// #define FIX1  // Do *not* use DefaultValue attribute, taken in to account serialising but not de-serialising.
//// #define FIX2  // Set default value of variable to same as DefaultValue attribute.

namespace AsymmetricXmlSerializer
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Class to be serialized and demo point for asymmetric XmlSerializer behaviour.
    /// </summary>
    [Serializable(), XmlRoot(Namespace = "")]
    public class Clue
    {
        /// <summary>
        /// Comment out DefaultValue attribute and it will succeed.
        /// </summary>
        /// <remarks>Set variable to 'Nothing To See Here'to to work around.</remarks>
#if !FIX1
        [DefaultValue("Nothing To See Here")]
#endif
#if !FIX2
        public string text;
#else
        public string text = "Nothing To See Here";
#endif
    }

    /// <summary>
    /// Demo program for asymmetric XmlSerializer behaviour with DefaultValueAttribute.
    /// </summary>
    internal sealed class Program
    {
        public static void Main(string[] args)
        {
            // If initial value is the same as DefaultValueAttribute for variable declaration, 
            // it is not persisted by XmlSerializer.
            Clue clue = new Clue();
            clue.text = "Nothing To See Here";

            // Buffer for writing.
            StringBuilder buffer = new StringBuilder();
            
            // Don't need namespaces.
            XmlSerializerNamespaces no_namespaces = new XmlSerializerNamespaces();
            no_namespaces.Add(string.Empty, string.Empty);

            // Don't need xml declaration.
            XmlWriterSettings writer_settings = new XmlWriterSettings() { OmitXmlDeclaration = true };

            // Serialise the clue...
            XmlSerializer xml_serializer = new XmlSerializer(typeof(Clue));
            using (StringWriter string_writer = new StringWriter(buffer))
            {
                using (XmlWriter xml_writer = XmlWriter.Create(string_writer, writer_settings))
                {
                    xml_serializer.Serialize(xml_writer, clue, no_namespaces);
                }
            }

            string serialised = buffer.ToString();

            // Show serialised version of the clue...
            Console.WriteLine("Clue text value: {0}", clue.text);
            Console.WriteLine("Clue persisted:  {0}", buffer.ToString());

            // Deserialise the clue...
            Clue clue_read;
            using (StringReader sr = new StringReader(serialised))
            {
                clue_read = (Clue)xml_serializer.Deserialize(sr);
            }

            Debug.Assert(clue.text == clue_read.text, "Written & read clue's text should match");

            // Show clue 2 on console
            // Reset and reuse write buffer.
            buffer.Length = 0;
            using (StringWriter string_writer = new StringWriter(buffer))
            {
                using (XmlWriter xml_writer = XmlWriter.Create(string_writer, writer_settings))
                {
                    xml_serializer.Serialize(xml_writer, clue_read, no_namespaces);
                    Console.WriteLine("Read Clue text value: {0}", clue_read.text);
                    Console.WriteLine("Read Clue: {0}", buffer.ToString());
                }
            }
        }
    }
}
