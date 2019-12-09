using System;
using System.Collections.Generic;
using System.Linq;
using FaceRecognition.Back.Api.Enums;
using Microsoft.EntityFrameworkCore.Internal;

namespace FaceRecognition.Back.Api.Dictionaries
{
    public class MimeTypeDictionary : Dictionary<MimeType, string>
    {
        public static MimeTypeDictionary Defaults { get; }
        
        static MimeTypeDictionary()
        {
            Defaults = new MimeTypeDictionary();
            SeedDefaults();
        }
        
        private static void SeedDefaults()
        {
            Defaults.CreateType(MimeType.PNG, "png");
            Defaults.CreateType(MimeType.JPEG, "jpeg");
        }
        
        private void CreateType(MimeType mimeType, string type)
        {
            Add(mimeType, type);
        }

        public static string GetType(MimeType mimeType)
        {
            var exists = Defaults.TryGetValue(mimeType, out var type);
            if (!exists || type == null) throw new NotSupportedException(nameof(mimeType));
            return type;
        }

        public static MimeType GetMimeType(string type)
        {
            var mimeType = Defaults.FirstOrDefault(x => x.Value == type).Key;
            if (mimeType == MimeType.NONE) throw new NotSupportedException(nameof(type));

            return mimeType;
        }
    }
}