// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoHelpdeskChatBot.Models
{
    [Table("ResetPassword")]
    public partial class ResetPassword
    {
        [Key]
        public int Id { get; set; }

        public string EmailAddress { get; set; }

        public long? MobileNumber { get; set; }

        public int? PassCode { get; set; }

        public string TempPassword { get; set; }
    }
}
