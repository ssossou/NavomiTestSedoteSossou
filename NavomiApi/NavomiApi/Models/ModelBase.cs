using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NavomiApi.Models
{ 
    public class ModelBase
    {
        /// <summary>
        /// Validation errors set by the Validate method
        /// </summary>
        public string ValidationErrors { get; set; }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Validates the object using DataAnnotations
        /// </summary>
        /// <returns>True if validation succeeds</returns>
        virtual public bool Validate()
        {
            var validationContext = new ValidationContext(this);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(this, validationContext, validationResults, true);

            if(!isValid)
            {
                this.ValidationErrors = String.Join(" ", validationResults);
            }
            
            return isValid;
        }
    }
}
