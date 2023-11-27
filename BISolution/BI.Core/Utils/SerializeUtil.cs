using MemoryPack;
using MemoryPack.Compression;

namespace BI.Core.Utils
{
    public static class SerializeUtil
    {
        public static byte[] Serialize<T>(T data)
        {
            using var compressor = new BrotliCompressor();
            MemoryPackSerializer.Serialize(compressor, data);

            return compressor.ToArray();
        }

        public static T Deserialize<T>(byte[] dataBytes)
        {
            using var deCompressor = new BrotliDecompressor(); 
            var decompressedBuffer = deCompressor.Decompress(dataBytes);
            
            return MemoryPackSerializer.Deserialize<T>(decompressedBuffer);

        }
    }
}
