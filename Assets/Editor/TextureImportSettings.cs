using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityGameMining.Editor
{
    /// <summary>
    /// 图片导入后处理：仅对 Assets/Art/UI 路径下的图片生效
    /// - 设为 Sprite 模式
    /// - 取消 Mip Maps
    /// - MaxSize 设为大于图片尺寸的最小 2 的幂，上限 1024
    /// - Android 平台压缩格式设为 ASTC 6x6 Block
    /// - grid 开头的图片自动设置九宫格边框
    /// </summary>
    public class TextureImportSettings : AssetPostprocessor
    {
        [Serializable]
        private sealed class NineSliceBorder
        {
            public int left;
            public int bottom;
            public int right;
            public int top;
        }

        private void OnPreprocessTexture()
        {
            if (Path.GetFileNameWithoutExtension(assetPath).EndsWith("_9s", StringComparison.OrdinalIgnoreCase)
                && TryReadNineSliceBorder(assetPath, out NineSliceBorder border))
            {
                var slicedImporter = (TextureImporter)assetImporter;
                slicedImporter.textureType = TextureImporterType.Sprite;
                slicedImporter.spriteImportMode = SpriteImportMode.Single;
                slicedImporter.alphaIsTransparency = true;
                slicedImporter.mipmapEnabled = false;
                slicedImporter.spriteBorder = new Vector4(border.left, border.bottom, border.right, border.top);
                if (!assetPath.StartsWith("Assets/Art/UI/", StringComparison.Ordinal))
                    return;
            }
            if (!assetPath.StartsWith("Assets/Art/UI/", StringComparison.Ordinal))
                return;

            var importer = (TextureImporter)assetImporter;

            // 设为 Sprite（单图）
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;

            // 取消 Mip Maps
            importer.mipmapEnabled = false;

            // MaxSize 设为大于图片尺寸的最小 2 的幂，上限 1024
            if (TryReadImageSize(assetPath, out int width, out int height))
            {
                int maxDimension = Mathf.Max(width, height);
                importer.maxTextureSize = Mathf.Min(Mathf.NextPowerOfTwo(maxDimension), 1024);

                if (Path.GetFileNameWithoutExtension(assetPath).StartsWith("grid", StringComparison.OrdinalIgnoreCase)
                    && !Path.GetFileNameWithoutExtension(assetPath).EndsWith("_9s", StringComparison.OrdinalIgnoreCase))
                {
                    int left = width / 2;
                    int top = height / 2;
                    importer.spriteBorder = new Vector4(left, top, width-left, height-top);
                }
            }
            else
            {
                importer.maxTextureSize = 1024;
            }

            // Android 平台压缩格式 ASTC 6x6
            var platformSettings = new TextureImporterPlatformSettings
            {
                name = "Android",
                overridden = true,
                format = TextureImporterFormat.ASTC_6x6,
                maxTextureSize = importer.maxTextureSize
            };

            importer.SetPlatformTextureSettings(platformSettings);
        }

        // Read the exporter's PNG tEXt chunk without creating a companion asset.
        private static bool TryReadNineSliceBorder(string path, out NineSliceBorder border)
        {
            border = null;
            using var stream = File.OpenRead(path);
            using var reader = new BinaryReader(stream);
            byte[] signature = reader.ReadBytes(8);
            if (signature.Length != 8 || signature[0] != 137 || signature[1] != 80 ||
                signature[2] != 78 || signature[3] != 71 || signature[4] != 13 ||
                signature[5] != 10 || signature[6] != 26 || signature[7] != 10)
                return false;
            while (stream.Length - stream.Position >= 12)
            {
                byte[] size = reader.ReadBytes(4);
                uint length = ((uint)size[0] << 24) | ((uint)size[1] << 16) | ((uint)size[2] << 8) | size[3];
                string type = System.Text.Encoding.ASCII.GetString(reader.ReadBytes(4));
                if (length > stream.Length - stream.Position - 4)
                    return false;
                if (type == "tEXt" && length <= 4096)
                {
                    string text = System.Text.Encoding.UTF8.GetString(reader.ReadBytes((int)length));
                    const string prefix = "UnityNineSlice\0";
                    if (text.StartsWith(prefix, StringComparison.Ordinal))
                    {
                        border = JsonUtility.FromJson<NineSliceBorder>(text.Substring(prefix.Length));
                        return border != null;
                    }
                }
                else
                    stream.Seek(length, SeekOrigin.Current);
                stream.Seek(4, SeekOrigin.Current); // CRC
                if (type == "IEND")
                    break;
            }
            return false;
        }

        /// <summary>
        /// 读取 PNG/JPEG 文件头获取图片尺寸，不加载完整图片
        /// </summary>
        private static bool TryReadImageSize(string path, out int width, out int height)
        {
            width = 0;
            height = 0;

            try
            {
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] header = new byte[2];
                fs.Read(header, 0, 2);

                // JPEG: FF D8
                if (header[0] == 0xFF && header[1] == 0xD8)
                {
                    while (fs.Position < fs.Length - 1)
                    {
                        fs.Read(header, 0, 2);
                        if (header[0] != 0xFF)
                            continue;

                        byte marker = header[1];
                        // SOF markers
                        if (marker == 0xC0 || marker == 0xC1 || marker == 0xC2)
                        {
                            byte[] sof = new byte[7];
                            fs.Read(sof, 0, 7);
                            height = (sof[1] << 8) | sof[2];
                            width = (sof[3] << 8) | sof[4];
                            return true;
                        }

                        // Skip variable-length segments
                        if (marker >= 0xC0 && marker <= 0xFE && marker != 0xC4 && marker != 0xC8 && marker != 0xCC)
                            continue;

                        byte[] len = new byte[2];
                        fs.Read(len, 0, 2);
                        int skip = ((len[0] << 8) | len[1]) - 2;
                        if (skip > 0)
                            fs.Seek(skip, SeekOrigin.Current);
                    }
                    return false;
                }

                // PNG: check 8-byte signature
                byte[] signature = new byte[6];
                fs.Read(signature, 0, 6);
                if (header[0] == 0x89 && header[1] == 0x50 &&
                    signature[0] == 0x4E && signature[1] == 0x47 &&
                    signature[2] == 0x0D && signature[3] == 0x0A &&
                    signature[4] == 0x1A && signature[5] == 0x0A)
                {
                    fs.Seek(8, SeekOrigin.Current); // skip IHDR length + tag
                    byte[] dims = new byte[8];
                    fs.Read(dims, 0, 8);
                    width  = (dims[0] << 24) | (dims[1] << 16) | (dims[2] << 8) | dims[3];
                    height = (dims[4] << 24) | (dims[5] << 16) | (dims[6] << 8) | dims[7];
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

    }
}
