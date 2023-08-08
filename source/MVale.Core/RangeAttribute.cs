// using System;
// using System.Collections;
// using System.ComponentModel.DataAnnotations;

// namespace MVale.Core.Attributes
// {
//     public sealed class RangeAttribute : ValidationAttribute
//     {
//         public Type Type { get; }

//         public object Min { get; }

//         public object Max { get; }

//         public RangeAttribute(Type type, string min, string max)
//         {
//             if (Type == null)
//                 throw new ArgumentNullException(nameof(type));

//             if (min == null && max == null)
//                 throw new InvalidOperationException(
//                     $"Parameters '{nameof(min)}' and '{nameof(max)}' must not be both null.");

//             Type = type;
//             Min = min;
//             Max = max;
//         }

//         protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//         {
//             return base.IsValid(value, validationContext);
//         }
//     }
// }