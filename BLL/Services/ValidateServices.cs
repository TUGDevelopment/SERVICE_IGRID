using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ValidateServices
    {
        public static List<ValidationResult> CheckRequired(object instance, Type metaDataType)
        {
            var listOfErrors = new List<ValidationResult>();

            var T = instance.GetType();

            TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(T, metaDataType), T);

            var validationContext = new ValidationContext(instance, null, null);

            var isValid = Validator.TryValidateObject(instance, validationContext, listOfErrors, true);

            return listOfErrors;
        }
    }
}
