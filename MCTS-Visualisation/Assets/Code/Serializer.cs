using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MCTS.Visualisation
{
    /// <summary>
    /// Conveinience class used to serialize and deserialize data for transmission across networks
    /// </summary>
    static class Serializer
    {
        /// <summary>
        /// Serialize the passed in object and output a byte array representation of it
        /// </summary>
        /// <param name="o">The object to serialize</param>
        /// <returns>A byte array which represents the passed in object</returns>
        public static byte[] Serialize(object o)
        {
            using (var memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, o);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Deserializes a byte array representing an object into an instance of an object
        /// </summary>
        /// <param name="serializedData">The byte array representing the serialized object</param>
        /// <returns>A deserialized instance of an object</returns>
        public static object Deserialize(byte[] serializedData)
        {
            using (var memoryStream = new MemoryStream(serializedData))
            {
                return new BinaryFormatter().Deserialize(memoryStream);
            }
        }
    }
}
