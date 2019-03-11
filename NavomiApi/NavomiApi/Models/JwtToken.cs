using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace NavomiApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class JwtToken : ModelBase
    {
        /// <summary>
        /// JWT Token string
        /// </summary>
        [DataMember(Name = "token")]
        [Required]
        public string Token { get; set; }


        /// <summary>
        /// JWT Token expiration
        /// </summary>
        [DataMember(Name = "validTo")]
        [Required]
        public DateTime ValidTo {
            get;
            set;
        }

    }
}
