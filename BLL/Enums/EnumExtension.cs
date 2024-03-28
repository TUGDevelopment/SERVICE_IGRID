using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BLL.Enums.EnumAttribute;

namespace BLL.Enums
{

    /// <summary>
    /// The enum attrribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumAttribute"/> class.
        /// </summary>
        protected EnumAttribute()
        {
        }


        public class MappingAttribute : EnumAttribute
        {
            public string PackagingTypeKey { get; set; }
            public string PackagingType { get; set; }
            public string BomItemNonMulti { get; set; }
            public string BomItemMulti { get; set; }

            public MappingAttribute()
            {
            }

            public MappingAttribute(string packagingTypeKey, string packagingType, string bomItemNonMulti, string bomItemMulti)
            {
                PackagingTypeKey = packagingTypeKey;
                PackagingType = packagingType;
                BomItemNonMulti = bomItemNonMulti;
                BomItemMulti = bomItemMulti;
            }            
        }
    }

    /// <summary>
    /// The enum extension.
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// The get attribute.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="EnumAttribute"/>.
        /// </returns>
        public static EnumAttribute GetAttribute(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            var atts = (EnumAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumAttribute), false);
            return atts.Length > 0 ? atts[0] : null;
        }

        public static string GetAttributePackagingTypeKey(this Enum value)
        {
            return ((MappingAttribute)value.GetAttribute()).PackagingTypeKey;
        }

        public static string GetAttributePackagingType(this Enum value)
        {
            return ((MappingAttribute)value.GetAttribute()).PackagingType;
        }

        public static string GetAttributeBomItemNonMulti(this Enum value)
        {
            return ((MappingAttribute)value.GetAttribute()).BomItemNonMulti;
        }

        public static string GetAttributeBomItemMulti(this Enum value)
        {
            return ((MappingAttribute)value.GetAttribute()).BomItemMulti;
        }
    }

    public static class EnumExtension<T>
    {
        public static T GetEnumByPackagingType(string packagingType)
        {
            var enumType = typeof(T);

            var items = enumType.GetMembers(BindingFlags.Public | BindingFlags.Static).Where(e => e.Name == packagingType);

            return (T)Enum.Parse(typeof(T), items.FirstOrDefault().Name, true);
        }
    }
}
