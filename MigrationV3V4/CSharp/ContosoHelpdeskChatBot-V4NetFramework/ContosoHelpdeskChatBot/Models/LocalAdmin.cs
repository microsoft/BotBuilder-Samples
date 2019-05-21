// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoHelpdeskChatBot.Models
{
    [Table("LocalAdmin")]
    public partial class LocalAdmin
    {
        [Key]
        public int Id { get; set; }

        public string MachineName { get; set; }

        public int? AdminDuration { get; set; }
    }
}
